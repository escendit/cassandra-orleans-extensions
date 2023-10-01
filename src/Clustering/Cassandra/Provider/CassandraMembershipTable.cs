// Copyright (c) Escendit Ltd. All Rights Reserved.
// Licensed under the MIT. See LICENSE.txt file in the solution root for full license information.

#pragma warning disable CA1812

namespace Escendit.Orleans.Clustering.Cassandra;

using Escendit.Extensions.Hosting.Cassandra;
using global::Cassandra;
using global::Orleans;
using global::Orleans.Configuration;
using global::Orleans.Runtime;
using Microsoft.Extensions.Logging;

/// <summary>
/// Cassandra Membership Table.
/// </summary>
internal sealed class CassandraMembershipTable : SessionContextProviderBase, IMembershipTable
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CassandraMembershipTable"/> class.
    /// </summary>
    /// <param name="name">The name (of client).</param>
    /// <param name="logger">The logger.</param>
    /// <param name="cluster">The cluster.</param>
    /// <param name="clientOptions">The client options.</param>
    /// <param name="clusterOptions">The cluster options.</param>
    public CassandraMembershipTable(
        string name,
        ICluster cluster,
        ILogger<CassandraMembershipTable> logger,
        CassandraClientOptions clientOptions,
        ClusterOptions clusterOptions)
        : base(name, logger, cluster, clientOptions, clusterOptions)
    {
    }

    /// <summary>
    /// Finalizes an instance of the <see cref="CassandraMembershipTable"/> class.
    /// </summary>
    ~CassandraMembershipTable() => Dispose(false);

    /// <inheritdoc/>
    public Task InitializeMembershipTable(bool tryInitTableVersion)
        => Initialize(tryInitTableVersion);

    /// <inheritdoc/>
    public Task DeleteMembershipTableEntries(string clusterId)
        => DeleteMemberships(clusterId);

    /// <inheritdoc/>
    public Task CleanupDefunctSiloEntries(DateTimeOffset beforeDate)
        => CleanupDefunctMembers(beforeDate);

    /// <inheritdoc/>
    public Task<MembershipTableData> ReadRow(SiloAddress key)
        => ReadOne(key);

    /// <inheritdoc/>
    public Task<MembershipTableData> ReadAll()
        => ReadMany();

    /// <inheritdoc/>
    public Task<bool> InsertRow(MembershipEntry entry, TableVersion tableVersion)
        => Insert(entry, tableVersion);

    /// <inheritdoc/>
    public Task<bool> UpdateRow(MembershipEntry entry, string etag, TableVersion tableVersion)
        => Update(entry, etag, tableVersion);

    /// <inheritdoc/>
    public Task UpdateIAmAlive(MembershipEntry entry)
        => Ping(entry);
}
