// Copyright (c) Escendit Ltd. All Rights Reserved.
// Licensed under the MIT. See LICENSE.txt file in the solution root for full license information.

#pragma warning disable CA1812

namespace Escendit.Orleans.Clustering.Cassandra;

using System.Reflection;
using System.Text;

using Escendit.Extensions.Hosting.Cassandra;
using global::Cassandra;
using global::Cassandra.Mapping;
using global::Orleans;
using global::Orleans.Configuration;
using global::Orleans.Runtime;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Options;

/// <summary>
/// Cassandra Membership Table.
/// </summary>
internal sealed partial class CassandraMembershipTable : IMembershipTable, IDisposable
{
    private readonly ILogger _logger;
    private readonly Assembly _selfAssembly;
    private readonly CassandraClientOptions _clientOptions;
    private readonly ClusterOptions _clusterOptions;
    private readonly ICluster _cluster;
    private ISession? _session;
    private IMapper? _mapper;

    /// <summary>
    /// Initializes a new instance of the <see cref="CassandraMembershipTable"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="serviceProvider">The service provider.</param>
    /// <param name="options">The options.</param>
    public CassandraMembershipTable(
        ILogger<CassandraMembershipTable> logger,
        IServiceProvider serviceProvider,
        IOptions<CassandraClusteringOptions> options)
    {
        ArgumentNullException.ThrowIfNull(serviceProvider);
        ArgumentNullException.ThrowIfNull(options);
        _logger = logger;
        _clusterOptions = serviceProvider.GetRequiredService<IOptions<ClusterOptions>>().Value;
        _cluster = serviceProvider.GetRequiredServiceByName<ICluster>(options.Value.ClientName);
        _clientOptions = serviceProvider.GetOptionsByName<CassandraClientOptions>(options.Value.ClientName);
        _selfAssembly = Assembly.GetAssembly(typeof(CassandraMembershipTable))!;
    }

    /// <summary>
    /// Finalizes an instance of the <see cref="CassandraMembershipTable"/> class.
    /// </summary>
    ~CassandraMembershipTable() => Dispose(false);

    /// <inheritdoc/>
    public async Task InitializeMembershipTable(bool tryInitTableVersion)
    {
        var mappingConfiguration = new MappingConfiguration()
            .Define<MembershipMapping>();
        _session = await _cluster
            .ConnectAsync(string.Empty)
            .ConfigureAwait(false);
        _session.CreateKeyspaceIfNotExists(_clientOptions.DefaultKeyspace);
        _session.ChangeKeyspace(_clientOptions.DefaultKeyspace);
        _mapper = new Mapper(_session, mappingConfiguration);

        await InitializeDatabaseAsync()
            .ConfigureAwait(false);

        if (tryInitTableVersion)
        {
            await InitializeVersionDataAsync(_clusterOptions.ClusterId)
                .ConfigureAwait(false);
        }

        await _session.UserDefinedTypes.DefineAsync(UdtMap
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
            .Map(p => p.UpdateZone, "update_zone"));
        await _session.UserDefinedTypes.DefineAsync(UdtMap
            .For<SuspectTime>("suspect_time", _session.Keyspace)
            .Map(p => p.Timestamp, "timestamp")
            .Map(p => p.Address, "address"))
            .ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task DeleteMembershipTableEntries(string clusterId)
    {
        ArgumentNullException.ThrowIfNull(clusterId);
        await _mapper!
            .DeleteAsync<Membership>("WHERE id = ?", clusterId)
            .ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task CleanupDefunctSiloEntries(DateTimeOffset beforeDate)
    {
        var membership = await ReadAll();
        var deadMembers = new List<string>();
        foreach (var silo in membership.Members.Where(p => p.Item1.Status == SiloStatus.Dead && p.Item1.IAmAliveTime < beforeDate))
        {
            deadMembers.Add(silo.Item1.SiloAddress.ToParsableString());
        }

        await _mapper!
            .UpdateIfAsync<Membership>("SET silos = silos - ? WHERE id = ?", deadMembers, _clusterOptions.ClusterId)
            .ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task<MembershipTableData> ReadRow(SiloAddress key)
    {
        ArgumentNullException.ThrowIfNull(key);
        var results = await _mapper!
            .FetchAsync<Membership>("WHERE id = ?", _clusterOptions.ClusterId)
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

        var matchedSilo = MatchSiloKey(result.Silos, key);
        var entry = MapEntry(matchedSilo);
        return new MembershipTableData(
            new Tuple<MembershipEntry, string>(entry, matchedSilo.Value.Etag),
            new TableVersion(result.Version, result.Etag));
    }

    /// <inheritdoc/>
    public async Task<MembershipTableData> ReadAll()
    {
        var results = await _mapper!
            .FetchAsync<Membership>("WHERE id = ?", _clusterOptions.ClusterId)
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

    /// <inheritdoc/>
    public async Task<bool> InsertRow(MembershipEntry entry, TableVersion tableVersion)
    {
        ArgumentNullException.ThrowIfNull(entry);
        ArgumentNullException.ThrowIfNull(tableVersion);
        var silos = await ReadAll();

        var currentEntry = silos.Members.FirstOrDefault(w => w.Item1.SiloAddress.ToParsableString() == entry.SiloAddress.ToParsableString());

        if (currentEntry is not null)
        {
            // already exists, fail.
            return false;
        }

        var appliedInfo = await _mapper!.UpdateIfAsync<Membership>(
            "SET silos[?] = ?, version = ?, etag = ? WHERE id = ?",
            entry.SiloAddress.ToParsableString(),
            MapSilo(entry, tableVersion),
            tableVersion.Version,
            tableVersion.VersionEtag,
            _clusterOptions.ClusterId)
            .ConfigureAwait(false);
        return appliedInfo.Applied;
    }

    /// <inheritdoc/>
    public async Task<bool> UpdateRow(MembershipEntry entry, string etag, TableVersion tableVersion)
    {
        ArgumentNullException.ThrowIfNull(entry);
        ArgumentNullException.ThrowIfNull(etag);
        ArgumentNullException.ThrowIfNull(tableVersion);
        var row = await ReadRow(entry.SiloAddress)
            .ConfigureAwait(false);

        if (!row.Members.Any())
        {
            return false;
        }

        var appliedInfo = await _mapper!.UpdateIfAsync<Membership>(
            "SET silos[?] = ?, version = ?, etag = ? WHERE id = ?",
            entry.SiloAddress.ToParsableString(),
            MapSilo(entry, etag),
            tableVersion.Version,
            tableVersion.VersionEtag,
            _clusterOptions.ClusterId)
            .ConfigureAwait(false);

        return appliedInfo.Applied;
    }

    /// <inheritdoc/>
    public async Task UpdateIAmAlive(MembershipEntry entry)
    {
        ArgumentNullException.ThrowIfNull(entry);
        var resultList = await _mapper!
            .FetchAsync<Membership>("WHERE id = ?", _clusterOptions.ClusterId)
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

        await _mapper!.UpdateAsync<Membership>(
                "SET silos[?] = ? WHERE id = ?",
                entry.SiloAddress.ToParsableString(),
                silo,
                _clusterOptions.ClusterId)
            .ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
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

    private void Dispose(bool disposing)
    {
        if (!disposing)
        {
            return;
        }

        _session?.Dispose();
        _cluster.Dispose();
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

    private async Task InitializeVersionDataAsync(string clusterId)
    {
        ArgumentNullException.ThrowIfNull(clusterId);
        try
        {
            await _mapper!
                .InsertIfNotExistsAsync(new Membership { Id = clusterId })
                .ConfigureAwait(false);
        }
        catch (InvalidCastException ex)
        {
            LogException(ex, ex.Message);
        }
    }

    [LoggerMessage(
        EventId = 500,
        EventName = "Log Exception",
        Level = LogLevel.Error,
        Message = "Error occurred: {message}")]
    private partial void LogException(Exception ex, string message);
}
