// Copyright (c) Escendit Ltd. All Rights Reserved.
// Licensed under the MIT. See LICENSE.txt file in the solution root for full license information.

namespace Escendit.Orleans.Clustering.Cassandra;

using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using Escendit.Extensions.Hosting.Cassandra;
using global::Cassandra;
using global::Cassandra.Mapping;
using global::Orleans;
using global::Orleans.Configuration;
using global::Orleans.Runtime;
using Microsoft.Extensions.Logging;

/// <summary>
/// Data Provider.
/// </summary>
internal partial class SessionContextProviderBase : IDisposable
{
    private readonly string _name;
    private readonly ILogger _logger;
    private readonly ICluster _cluster;
    private readonly CassandraClientOptions _clientOptions;
    private readonly ClusterOptions _clusterOptions;
    private readonly Assembly _selfAssembly;
    private ISession? _session;
    private IMapper? _mapper;

    /// <summary>
    /// Initializes a new instance of the <see cref="SessionContextProviderBase"/> class.
    /// </summary>
    /// <param name="name">The name [of client].</param>
    /// <param name="logger">The logger.</param>
    /// <param name="cluster">The cassandra cluster.</param>
    /// <param name="clientOptions">The client options.</param>
    /// <param name="clusterOptions">The cluster options.</param>
    protected SessionContextProviderBase(
        string name,
        ILogger logger,
        ICluster cluster,
        CassandraClientOptions clientOptions,
        ClusterOptions clusterOptions)
    {
        _name = name;
        _logger = logger;
        _cluster = cluster;
        _clientOptions = clientOptions;
        _clusterOptions = clusterOptions;
        _selfAssembly = typeof(SessionContextProviderBase).Assembly;
    }

    /// <summary>
    /// Finalizes an instance of the <see cref="SessionContextProviderBase"/> class.
    /// </summary>
    ~SessionContextProviderBase() => Dispose(false);

    /// <inheritdoc/>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Initialize the cluster.
    /// </summary>
    /// <param name="tryInitializeData">The try initialize data in the cluster.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    protected async Task Initialize(bool tryInitializeData = false)
    {
        LogInitialize(_name, _clientOptions.DefaultKeyspace!);
        var mappingConfiguration = new MappingConfiguration()
            .Define<MembershipMapping>();
        _session = await Execute(() => _cluster
                    .ConnectAsync(string.Empty))
            .ConfigureAwait(false);
        LogConnect(_name, string.Empty);
        _session.CreateKeyspaceIfNotExists(_clientOptions.DefaultKeyspace);
        _session.ChangeKeyspace(_clientOptions.DefaultKeyspace);
        LogConnect(_name, _clientOptions.DefaultKeyspace!);
        _mapper = new Mapper(_session, mappingConfiguration);

        await Execute(InitializeDatabaseAsync)
            .ConfigureAwait(false);

        if (tryInitializeData)
        {
            await Execute(() => InitializeVersionDataAsync(_clusterOptions.ClusterId))
                .ConfigureAwait(false);
        }

        await Execute(() => _session.UserDefinedTypes.DefineAsync(UdtMap
            .For<Silo>("silo", _session.Keyspace)
            .Map(p => p.FaultZone, "fault_zone")
            .Map(p => p.ProxyPort, "proxy_port")
            .Map(p => p.HostName, "host")
            .Map(p => p.StartedOn, "started_on")
            .Map(p => p.Address, "address")
            .Map(p => p.Etag, "etag")
            .Map(p => p.Name, "name")
            .Map(p => p.Role, "role")
            .Map(p => p.Status, "status")
            .Map(p => p.Timestamp, "timestamp")
            .Map(p => p.AliveOn, "alive_on")
            .Map(p => p.SuspectTimes, "suspect_times")
            .Map(p => p.UpdateZone, "update_zone")))
            .ConfigureAwait(false);
        await Execute(() => _session.UserDefinedTypes.DefineAsync(UdtMap
                .For<SuspectTime>("suspect_time", _session.Keyspace)
                .Map(p => p.Timestamp, "timestamp")
                .Map(p => p.Address, "address")))
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Delete Memberships.
    /// </summary>
    /// <param name="clusterId">The cluster id to delete from.</param>
    /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
    protected async Task DeleteMemberships(string clusterId)
    {
        ArgumentNullException.ThrowIfNull(clusterId);
        await Execute(() => _mapper!
                .DeleteAsync<Membership>("WHERE id = ?", clusterId))
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Cleanup Defunct Members.
    /// </summary>
    /// <param name="beforeDate">The before date.</param>
    /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
    protected async Task CleanupDefunctMembers(DateTimeOffset beforeDate)
    {
        var membership = await Execute(ReadMany)
            .ConfigureAwait(false);
        var deadMembers = membership
            .Members
            .Where(p =>
                p.Item1.Status == SiloStatus.Dead && p.Item1.IAmAliveTime < beforeDate)
            .Select(silo => silo.Item1.SiloAddress.ToParsableString())
            .ToList();

        await Execute(() => _mapper!
                .UpdateIfAsync<Membership>("SET silos = silos - ? WHERE id = ?", deadMembers, _clusterOptions.ClusterId))
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Read One Member.
    /// </summary>
    /// <param name="address">The silo address.</param>
    /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
    protected async Task<MembershipTableData> ReadOne(SiloAddress address)
    {
        ArgumentNullException.ThrowIfNull(address);
        var results = await Execute(() => _mapper!
                .FetchAsync<Membership>("WHERE id = ?", _clusterOptions.ClusterId))
            .ConfigureAwait(false);

        var memberships = results as Membership[] ?? results.ToArray();
        if (!memberships.Any())
        {
            return new MembershipTableData(
                new TableVersion(0, Guid.Empty.ToString()));
        }

        var result = memberships.FirstOrDefault();

        if (result is null)
        {
            return new MembershipTableData(
                new TableVersion(0, Guid.Empty.ToString()));
        }

        if (!result.Silos.Any())
        {
            return new MembershipTableData(
                new TableVersion(0, Guid.Empty.ToString()));
        }

        var matchedSilo = MatchSiloKey(result.Silos, address);
        var entry = MapEntry(matchedSilo);
        return new MembershipTableData(
            new Tuple<MembershipEntry, string>(entry, matchedSilo.Value.Etag),
            new TableVersion(result.Version, result.Etag));
    }

    /// <summary>
    /// Read Many.
    /// </summary>
    /// <returns>The membership table data.</returns>
    protected async Task<MembershipTableData> ReadMany()
    {
        var results = await Execute(() => _mapper!
                .FetchAsync<Membership>("WHERE id = ?", _clusterOptions.ClusterId))
            .ConfigureAwait(false);

        var result = results.FirstOrDefault();

        if (result is null)
        {
            return new MembershipTableData(
                new TableVersion(0, Guid.Empty.ToString()));
        }

        var entries = result
            .Silos
            .ToList()
            .ConvertAll(item => new Tuple<MembershipEntry, string>(
                MapEntry(item),
                item.Value.Etag));
        return new MembershipTableData(entries, new TableVersion(result.Version, result.Etag));
    }

    /// <summary>
    /// Insert One.
    /// </summary>
    /// <param name="entry">The entry.</param>
    /// <param name="tableVersion">The table version.</param>
    /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
    protected async Task<bool> Insert(MembershipEntry entry, TableVersion tableVersion)
    {
        ArgumentNullException.ThrowIfNull(entry);
        ArgumentNullException.ThrowIfNull(tableVersion);
        var silos = await Execute(ReadMany);

        var currentEntry = silos.Members.FirstOrDefault(w => w.Item1.SiloAddress.ToParsableString() == entry.SiloAddress.ToParsableString());

        if (currentEntry is not null)
        {
            // already exists, fail.
            return false;
        }

        var appliedInfo = await Execute(() => _mapper!
            .UpdateIfAsync<Membership>(
                "SET silos[?] = ?, version = ?, etag = ? WHERE id = ?",
                entry.SiloAddress.ToParsableString(),
                MapSilo(entry, tableVersion),
                tableVersion.Version,
                tableVersion.VersionEtag,
                _clusterOptions.ClusterId))
            .ConfigureAwait(false);
        return appliedInfo.Applied;
    }

    /// <summary>
    /// Update One.
    /// </summary>
    /// <param name="entry">The entry.</param>
    /// <param name="etag">The etag.</param>
    /// <param name="tableVersion">The table version.</param>
    /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
    protected async Task<bool> Update(MembershipEntry entry, string etag, TableVersion tableVersion)
    {
        ArgumentNullException.ThrowIfNull(entry);
        ArgumentNullException.ThrowIfNull(etag);
        ArgumentNullException.ThrowIfNull(tableVersion);
        var row = await Execute(() =>
                ReadOne(entry.SiloAddress))
            .ConfigureAwait(false);

        if (!row.Members.Any())
        {
            return false;
        }

        var appliedInfo = await Execute(() => _mapper!
            .UpdateIfAsync<Membership>(
                "SET silos[?] = ?, version = ?, etag = ? WHERE id = ?",
                entry.SiloAddress.ToParsableString(),
                MapSilo(entry, etag),
                tableVersion.Version,
                tableVersion.VersionEtag,
                _clusterOptions.ClusterId))
            .ConfigureAwait(false);

        return appliedInfo.Applied;
    }

    /// <summary>
    /// Ping. Pong. (IAmAlive).
    /// </summary>
    /// <param name="entry">The entry.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    protected async Task Ping(MembershipEntry entry)
    {
        ArgumentNullException.ThrowIfNull(entry);
        var resultList = await Execute(() => _mapper!
                .FetchAsync<Membership>("WHERE id = ?", _clusterOptions.ClusterId))
            .ConfigureAwait(false);

        var results = resultList.ToList();
        if (!results.Any())
        {
            return;
        }

        var result = results.FirstOrDefault();

        if (result is null)
        {
            return;
        }

        var siloKey = entry.SiloAddress.ToParsableString();
        var siloExists = result.Silos.ContainsKey(siloKey);

        if (!siloExists)
        {
            return;
        }

        var silo = result.Silos[siloKey];
        silo.AliveOn = entry.IAmAliveTime;

        await Execute(() => _mapper!
                .UpdateAsync<Membership>(
                    "SET silos[?] = ? WHERE id = ?",
                    entry.SiloAddress.ToParsableString(),
                    silo,
                    _clusterOptions.ClusterId))
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Get Gateway List.
    /// </summary>
    /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
    protected async Task<IList<Uri>> GetGatewayList()
    {
        var membershipTableData = await Execute(ReadMany);
        return membershipTableData
            .Members
            .Where(w => w.Item1.Status == SiloStatus.Active)
            .ToList()
            .ConvertAll(item => item
                .Item1
                .SiloAddress.ToGatewayUri());
    }

    /// <summary>
    /// Dispose.
    /// </summary>
    /// <param name="disposing">The disposing.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!disposing)
        {
            return;
        }

        _session?.Dispose();
        _cluster.Dispose();
    }

    private static KeyValuePair<string, Silo> MatchSiloKey(IDictionary<string, Silo> result, SiloAddress siloAddress)
    {
        return result
            .FirstOrDefault(w => w.Key == siloAddress.ToParsableString());
    }

    private static MembershipEntry MapEntry(KeyValuePair<string, Silo> entry)
    {
        return new MembershipEntry
        {
            SiloAddress = SiloAddress.FromParsableString(entry.Value.Address),
            Status = (SiloStatus)entry.Value.Status,
            FaultZone = entry.Value.FaultZone,
            SuspectTimes = entry
                .Value
                .SuspectTimes
                .ToList()
                .ConvertAll(item =>
                    new Tuple<SiloAddress, DateTime>(SiloAddress.FromParsableString(item.Address), item.Timestamp)),
            HostName = entry.Value.HostName,
            ProxyPort = entry.Value.ProxyPort,
            RoleName = entry.Value.Role,
            SiloName = entry.Value.Name,
            StartTime = entry.Value.StartedOn.DateTime,
            UpdateZone = entry.Value.UpdateZone,
            IAmAliveTime = entry.Value.AliveOn.DateTime,
        };
    }

    private static Silo MapSilo(MembershipEntry entry, TableVersion tableVersion)
    {
        return MapSilo(entry, tableVersion.VersionEtag);
    }

    private static Silo MapSilo(MembershipEntry entry, string etag)
    {
        return new Silo
        {
            Address = entry.SiloAddress.ToParsableString(),
            HostName = entry.HostName,
            ProxyPort = entry.ProxyPort,
            Etag = etag,
            Name = entry.SiloName,
            Role = entry.RoleName,
            SuspectTimes = entry.SuspectTimes is null
                ? new List<SuspectTime>()
                : entry
                    .SuspectTimes
                    .ConvertAll(item => new SuspectTime
                    {
                        Address = item.Item1.ToParsableString(),
                        Timestamp = item.Item2,
                    }),
            Status = (int)entry.Status,
            FaultZone = entry.FaultZone,
            AliveOn = entry.IAmAliveTime,
            StartedOn = entry.StartTime,
            UpdateZone = entry.UpdateZone,
            Timestamp = DateTime.UtcNow,
        };
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

    private async Task InitializeVersionDataAsync(string clusterId)
    {
        ArgumentNullException.ThrowIfNull(clusterId);
        try
        {
            await Execute(() => _mapper!
                    .InsertIfNotExistsAsync(new Membership { Id = clusterId }))
                .ConfigureAwait(false);
        }
        catch (InvalidCastException ex)
        {
            LogException(ex, ex.Message);
        }
    }

    private async Task InitializeDatabaseAsync()
    {
        var suspectTimesTypeStream = _selfAssembly.GetManifestResourceStream("Escendit.Orleans.Clustering.Cassandra.Resources.SQL.1_SuspectTime.sql");
        var siloTypeStream = _selfAssembly.GetManifestResourceStream("Escendit.Orleans.Clustering.Cassandra.Resources.SQL.2_Silo.sql");
        var membershipTableStream = _selfAssembly.GetManifestResourceStream("Escendit.Orleans.Clustering.Cassandra.Resources.SQL.3_Membership.sql");

        using var suspectTimesTypeReader = new StreamReader(suspectTimesTypeStream!, Encoding.UTF8, true);
        using var siloTypeReader = new StreamReader(siloTypeStream!, Encoding.UTF8, true);
        using var membershipTableReader = new StreamReader(membershipTableStream!, Encoding.UTF8, true);
        var sqlSuspectTimesType = await suspectTimesTypeReader.ReadToEndAsync().ConfigureAwait(false);
        var sqlSiloType = await siloTypeReader.ReadToEndAsync().ConfigureAwait(false);
        var sqlMembershipTable = await membershipTableReader.ReadToEndAsync().ConfigureAwait(false);

        await _session!.ExecuteAsync(new SimpleStatement(sqlSuspectTimesType));
        await _session!.ExecuteAsync(new SimpleStatement(sqlSiloType));
        await _session!.ExecuteAsync(new SimpleStatement(sqlMembershipTable));
    }

    [LoggerMessage(
        EventId = 100,
        EventName = "Initialize",
        Level = LogLevel.Information,
        Message = "Initializing {name} to {keyspace}")]
    private partial void LogInitialize(string name, string keyspace);

    [LoggerMessage(
        EventId = 101,
        EventName = "Connect",
        Level = LogLevel.Information,
        Message = "Connected {name} to {keyspace}")]
    private partial void LogConnect(string name, string keyspace);

    [LoggerMessage(
        EventId = 200,
        EventName = "Execution",
        Level = LogLevel.Debug,
        Message = "Executing with client {name} > {returnType} {selfType}.{action} completed in {elapsed}")]
    private partial void LogExecute(string name, string returnType, string selfType, string action, long elapsed);

    [LoggerMessage(
        EventId = 500,
        EventName = "Log Exception",
        Level = LogLevel.Error,
        Message = "Error occurred: {message}")]
    private partial void LogException(Exception ex, string message);

    [LoggerMessage(
        EventId = 500,
        EventName = "Exception",
        Level = LogLevel.Error,
        Message = "Exception with client {name} > {returnType} {selfType}.{action} {message}")]
    private partial void LogException(string name, Exception exception, string message, string returnType, string selfType, string action);
}
