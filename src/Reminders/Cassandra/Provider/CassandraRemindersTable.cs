// Copyright (c) Escendit Ltd. All Rights Reserved.
// Licensed under the MIT. See LICENSE.txt file in the solution root for full license information.

namespace Escendit.Orleans.Reminders.Cassandra.Provider;

using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using Escendit.Extensions.Hosting.Cassandra;
using global::Cassandra;
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
    private readonly string _name;
    private readonly ILogger _logger;
    private readonly ICluster _cluster;
    private readonly CassandraClientOptions _clientOptions;
    private readonly Assembly _selfAssembly;
    private readonly MappingConfiguration _mapping;
    private ISession? _session;
    private IMapper? _mapper;
    private PreparedStatement? _hashLookup;

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
        _selfAssembly = typeof(CassandraRemindersTable).Assembly;
        _mapping = new MappingConfiguration().Define<ReminderMapping>();
    }

    /// <inheritdoc/>
    public async Task Init()
    {
        _session = await _cluster.ConnectAsync(string.Empty);
        _session.CreateKeyspaceIfNotExists(_clientOptions.DefaultKeyspace);
        _session.ChangeKeyspace(_clientOptions.DefaultKeyspace);
        _mapper = new Mapper(_session, _mapping);
        await InitializeDatabase();
        _hashLookup = await _session.PrepareAsync("SELECT * FROM reminders WHERE hash > ? AND hash <= ? ALLOW FILTERING");
    }

    /// <inheritdoc/>
    public async Task<ReminderTableData> ReadRows(GrainId grainId)
    {
        var (type, id) = GenerateDatabaseIds(grainId);
        var results = await _mapper!.FetchAsync<Reminder>("WHERE type = ? AND id = ?", type, id);
        if (results is null)
        {
            return new ReminderTableData();
        }

        return new ReminderTableData(results
            .ToList()
            .ConvertAll(item => new ReminderEntry
            {
                GrainId = grainId,
                ReminderName = item.Name,
                ETag = item.Etag,
                Period = TimeSpan.FromTicks(item.Period),
                StartAt = item.StartOn.UtcDateTime,
            }));
    }

    /// <inheritdoc/>
    public async Task<ReminderTableData> ReadRows(uint begin, uint end)
    {
        var bigBegin = Convert.ToInt64(begin);
        var bigEnd = Convert.ToInt64(end);

        var resultSet = await _session!.ExecuteAsync(_hashLookup!.Bind(bigBegin, bigEnd));
        if (resultSet.GetAvailableWithoutFetching() == 0)
        {
            return new ReminderTableData();
        }

        var items = new List<ReminderEntry>();
        foreach (var row in resultSet.GetRows())
        {
            items.Add(new ReminderEntry
            {
                GrainId = BuildGrainId(row.GetValue<byte[]>("type"), row.GetValue<byte[]>("id")),
                ReminderName = row.GetValue<string>("name"),
                StartAt = row.GetValue<DateTimeOffset>("start_on").UtcDateTime,
                Period = TimeSpan.FromTicks(row.GetValue<long>("period")),
                ETag = row.GetValue<string>("etag"),
            });
        }

        return new ReminderTableData(items);
    }

    /// <inheritdoc/>
    public async Task<ReminderEntry> ReadRow(GrainId grainId, string reminderName)
    {
        ArgumentNullException.ThrowIfNull(reminderName);
        var (type, id) = GenerateDatabaseIds(grainId);
        var results
            = await _mapper!
                .FetchAsync<Reminder>("WHERE type = ? AND id = ? AND name = ?", type, id, reminderName);

        if (results is null)
        {
            return new ReminderEntry();
        }

        var items = results.ToList();

        if (!items.Any())
        {
            return new ReminderEntry();
        }

        var item = items.First()!;
        return new ReminderEntry
        {
            GrainId = grainId,
            ReminderName = reminderName,
            ETag = item.Etag,
            Period = TimeSpan.FromTicks(item.Period),
            StartAt = item.StartOn.UtcDateTime,
        };
    }

    /// <inheritdoc/>
    public async Task<string> UpsertRow(ReminderEntry entry)
    {
        ArgumentNullException.ThrowIfNull(entry);
        var etag = Encoding.UTF8.GetString(GrainIdKeyExtensions.CreateGuidKey(Guid.NewGuid()).Value.ToArray());
        var (type, id) = GenerateDatabaseIds(entry.GrainId);

        var row = new Reminder(
            type,
            id,
            entry.GrainId.GetUniformHashCode(),
            entry.ReminderName,
            new DateTimeOffset(entry.StartAt),
            entry.Period.Ticks,
            etag);

        await _mapper!.InsertAsync(row);
        return etag;
    }

    /// <inheritdoc/>
    public async Task<bool> RemoveRow(GrainId grainId, string reminderName, string eTag)
    {
        ArgumentNullException.ThrowIfNull(reminderName);
        ArgumentNullException.ThrowIfNull(eTag);
        var info = await _mapper!.DeleteIfAsync<Reminder>("WHERE type = ? AND id = ? AND name = ? AND etag = ?");
        return info.Applied;
    }

    /// <inheritdoc/>
    public Task TestOnlyClearTable()
    {
        return _mapper!.DeleteAsync<Reminder>("1=1");
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
        var remindersTableStream =
            _selfAssembly.GetManifestResourceStream(
                "Escendit.Orleans.Reminders.Cassandra.Resources.SQL.1_Reminders.sql");

        using var remindersTableReader = new StreamReader(remindersTableStream!, Encoding.UTF8, true);
        var sqlRemindersTable = await remindersTableReader.ReadToEndAsync().ConfigureAwait(false);

        await _session!.ExecuteAsync(new SimpleStatement(sqlRemindersTable));
    }

    /// <summary>
    /// Execute.
    /// </summary>
    /// <param name="action">The action.</param>
    /// <param name="actionName">The action name.</param>
    /// <returns>The task.</returns>
    private Task Execute(Func<Task> action, [CallerMemberName] string actionName = default!)
    {
        ArgumentNullException.ThrowIfNull(action);
        return Execute(
            async () =>
            {
                await action();
                return true;
            },
            actionName);
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
            stopwatch.Stop();
            LogExecute(_name, returnTypeName, selfTypeName, actionName, stopwatch.ElapsedMilliseconds);
            return result;
        }
        catch (Exception ex)
        {
            LogException(_name, ex, ex.Message, returnTypeName, selfTypeName, actionName);
            throw;
        }
    }

    [LoggerMessage(
        EventId = 200,
        EventName = "Execution",
        Level = LogLevel.Debug,
        Message = "Executing with client {name} > {returnType} {selfType}.{action} completed in {elapsed}")]
    private partial void LogExecute(string name, string returnType, string selfType, string action, long elapsed);

    [LoggerMessage(
        EventId = 500,
        EventName = "Exception",
        Level = LogLevel.Error,
        Message = "Exception with client {name} > {returnType} {selfType}.{action} {message}")]
    private partial void LogException(string name, Exception exception, string message, string returnType, string selfType, string action);
}
