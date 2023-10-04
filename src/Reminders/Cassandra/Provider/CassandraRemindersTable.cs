// Copyright (c) Escendit Ltd. All Rights Reserved.
// Licensed under the MIT. See LICENSE.txt file in the solution root for full license information.

namespace Escendit.Orleans.Reminders.Cassandra.Provider;

using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using Escendit.Extensions.Hosting.Cassandra;
using global::Cassandra;
using global::Cassandra.Data.Linq;
using global::Cassandra.Mapping;
using global::Orleans.Runtime;
using Mapping;
using Microsoft.Extensions.Logging;
using Schema;

/// <summary>
/// Cassandra Reminders Table.
/// </summary>
internal partial class CassandraRemindersTable : IReminderTable
{
    private const string RemindersTableName = "orleans_reminders";
    private readonly string _name;
    private readonly ILogger _logger;
    private readonly ICluster _cluster;
    private readonly CassandraClientOptions _clientOptions;
    private readonly MappingConfiguration _mapping;
    private ISession? _session;
    private PreparedStatement? _readRowsStatement;
    private PreparedStatement? _readRowsHashStatement;
    private PreparedStatement? _readRowStatement;
    private PreparedStatement? _upsertRowStatement;

    /// <summary>
    /// Initializes a new instance of the <see cref="CassandraRemindersTable"/> class.
    /// </summary>
    /// <param name="name">The name (of client).</param>
    /// <param name="logger">The logger.</param>
    /// <param name="cluster">The cluster.</param>
    /// <param name="clientOptions">The client options.</param>
    public CassandraRemindersTable(
        string name,
        ILogger<CassandraRemindersTable> logger,
        ICluster cluster,
        CassandraClientOptions clientOptions)
    {
        ArgumentNullException.ThrowIfNull(name);
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(cluster);
        ArgumentNullException.ThrowIfNull(clientOptions);
        _name = name;
        _logger = logger;
        _cluster = cluster;
        _clientOptions = clientOptions;
        _mapping = new MappingConfiguration()
            .Define<ReminderMapping>();
    }

    /// <inheritdoc/>
    public async Task Init()
    {
        _session = await _cluster.ConnectAsync(string.Empty);
        _session.CreateKeyspaceIfNotExists(_clientOptions.DefaultKeyspace);
        _session.ChangeKeyspace(_clientOptions.DefaultKeyspace);
        await InitializeDatabase();
        await InitializeStatements();
    }

    /// <inheritdoc/>
    public async Task<ReminderTableData> ReadRows(GrainId grainId)
    {
        var (type, id) = GenerateDatabaseIds(grainId);
        var resultSet = await Execute(() =>
            _session!.ExecuteAsync(_readRowsStatement!.Bind(type, id)));
        if (resultSet.GetAvailableWithoutFetching() == 0)
        {
            return new ReminderTableData();
        }

        return new ReminderTableData(resultSet
            .GetRows()
            .Select(MapReminderEntry)
            .AsEnumerable());
    }

    /// <inheritdoc/>
    public async Task<ReminderTableData> ReadRows(uint begin, uint end)
    {
        var bigBegin = Convert.ToInt64(begin);
        var bigEnd = Convert.ToInt64(end);

        var resultSet = await Execute(() =>
            _session!.ExecuteAsync(_readRowsHashStatement!.Bind(bigBegin, bigEnd)));
        if (resultSet.GetAvailableWithoutFetching() == 0)
        {
            return new ReminderTableData();
        }

        return new ReminderTableData(resultSet
            .GetRows()
            .Select(MapReminderEntry)
            .AsEnumerable());
    }

    /// <inheritdoc/>
    public async Task<ReminderEntry> ReadRow(GrainId grainId, string reminderName)
    {
        ArgumentNullException.ThrowIfNull(reminderName);
        var (type, id) = GenerateDatabaseIds(grainId);
        var resultSet = await Execute(() =>
            _session!.ExecuteAsync(_readRowStatement!.Bind(type, id, reminderName)));
        if (resultSet.GetAvailableWithoutFetching() == 0)
        {
            return new ReminderEntry();
        }

        return resultSet
            .GetRows()
            .Select(MapReminderEntry)
            .First();
    }

    /// <inheritdoc/>
    public async Task<string> UpsertRow(ReminderEntry entry)
    {
        ArgumentNullException.ThrowIfNull(entry);
        var etag = Encoding.UTF8.GetString(GrainIdKeyExtensions.CreateGuidKey(Guid.NewGuid()).Value.ToArray());
        var (type, id) = GenerateDatabaseIds(entry.GrainId);
        await Execute(() =>
            _session!
                .ExecuteAsync(_upsertRowStatement!
                    .Bind(
                        type,
                        id,
                        entry.ReminderName,
                        Convert.ToInt64(entry.GrainId.GetUniformHashCode()),
                        new DateTimeOffset(entry.StartAt),
                        entry.Period.Ticks,
                        etag)));
        return etag;
    }

    /// <inheritdoc/>
    public async Task<bool> RemoveRow(GrainId grainId, string reminderName, string eTag)
    {
        ArgumentNullException.ThrowIfNull(reminderName);
        ArgumentNullException.ThrowIfNull(eTag);
        var (type, id) = GenerateDatabaseIds(grainId);
        var resultSet = await Execute(() =>
            _session!.ExecuteAsync(new SimpleStatement(
                $"""
                DELETE FROM "{RemindersTableName}"
                WHERE type = ? AND id = ? AND name = ?
                IF etag = ?
                """,
                type,
                id,
                reminderName,
                eTag)));
        return resultSet.GetAvailableWithoutFetching() > 0;
    }

    /// <inheritdoc/>
    public Task TestOnlyClearTable()
    {
        return Execute(() => _session!.ExecuteAsync(new SimpleStatement($"""DELETE FROM "{RemindersTableName}";""")));
    }

    private static (byte[] Type, byte[] Id) GenerateDatabaseIds(GrainId grainId)
    {
        return (grainId.Type.Value.Value.ToArray(), grainId.Key.Value.ToArray());
    }

    private static GrainId BuildGrainId(byte[] type, byte[] id)
    {
        return GrainId.Create(new GrainType(type), new IdSpan(id));
    }

    private async Task InitializeDatabase()
    {
        await Execute(() => new Table<Reminder>(_session, _mapping, RemindersTableName)
            .CreateIfNotExistsAsync());
    }

    private async Task InitializeStatements()
    {
        _readRowsStatement = await Execute(() =>
            _session!.PrepareAsync(
                $"""
                SELECT type, id, name, start_on, period, etag
                FROM "{RemindersTableName}"
                WHERE type = ? AND id = ?
                ALLOW FILTERING
                """));
        _readRowsHashStatement = await Execute(() =>
            _session!.PrepareAsync(
                $"""
                SELECT type, id, name, start_on, period, etag
                FROM "{RemindersTableName}"
                WHERE hash > ? AND hash <= ?
                ALLOW FILTERING
                """));
        _readRowStatement = await Execute(() =>
            _session!.PrepareAsync(
                $"""
                SELECT type, id, name, start_on, period, etag
                FROM "{RemindersTableName}"
                WHERE type = ? AND id = ? AND name = ?
                """));
        _upsertRowStatement = await Execute(() =>
            _session!.PrepareAsync(
                $"""
                INSERT INTO "{RemindersTableName}" (type, id, name, hash, start_on, period, etag)
                VALUES (?, ?, ?, ?, ?, ?, ?)
                """));
    }

    /// <summary>
    /// Execute.
    /// </summary>
    /// <typeparam name="T">The type.</typeparam>
    /// <param name="action">The action.</param>
    /// <param name="actionName">The action name.</param>
    /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
    private Task<T> Execute<T>(Func<Task<T>> action, [CallerMemberName] string actionName = default!)
    {
        ArgumentNullException.ThrowIfNull(action);
        var returnType = typeof(T);
        var selfType = GetType();
        var returnTypeName = returnType.Name;
        var selfTypeName = selfType.Name;
        if (returnType.GenericTypeArguments.Any())
        {
            var arguments = string.Join(", ", returnType.GenericTypeArguments.ToList().ConvertAll(item => item.Name));
            returnTypeName = $"{returnTypeName}<{arguments}>";
        }

        try
        {
            var stopwatch = Stopwatch.StartNew();
            var result = action();
            result.ContinueWith(
                (_, _) =>
                {
                    stopwatch.Stop();
                    LogExecute(_name, returnTypeName, selfTypeName, actionName, stopwatch.ElapsedMilliseconds);
                },
                TaskContinuationOptions.OnlyOnRanToCompletion,
                TaskScheduler.Default);
            return result;
        }
        catch (Exception ex)
        {
            LogException(_name, ex, ex.Message, returnTypeName, selfTypeName, actionName);
            throw;
        }
    }

    private Task Execute(Func<Task> action, [CallerMemberName] string actionName = default!)
    {
        return Execute(
            async () =>
            {
                await action();
                return true;
            },
            actionName);
    }

    private ReminderEntry MapReminderEntry(Row row)
    {
        return new ReminderEntry
        {
            GrainId = BuildGrainId(row.GetValue<byte[]>("type"), row.GetValue<byte[]>("id")),
            ReminderName = row.GetValue<string>("name"),
            StartAt = row.GetValue<DateTimeOffset>("start_on").UtcDateTime,
            Period = TimeSpan.FromTicks(row.GetValue<long>("period")),
            ETag = row.GetValue<string>("etag"),
        };
    }

    [LoggerMessage(
        EventId = 200,
        EventName = "Execution",
        Level = LogLevel.Debug,
        Message = "Executing with client {name} > {returnType} {selfType}.{action} completed in {elapsed}ms")]
    private partial void LogExecute(string name, string returnType, string selfType, string action, long elapsed);

    [LoggerMessage(
        EventId = 500,
        EventName = "Exception",
        Level = LogLevel.Error,
        Message = "Exception with client {name} > {returnType} {selfType}.{action} {message}")]
    private partial void LogException(string name, Exception exception, string message, string returnType, string selfType, string action);
}
