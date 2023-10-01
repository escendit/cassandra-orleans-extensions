// Copyright (c) Escendit Ltd. All Rights Reserved.
// Licensed under the MIT. See LICENSE.txt file in the solution root for full license information.

namespace Escendit.Orleans.Clustering.Cassandra;

using Escendit.Extensions.Hosting.Cassandra;
using global::Cassandra;
using global::Orleans.Configuration;
using global::Orleans.Messaging;
using Microsoft.Extensions.Logging;
using Options;

/// <summary>
/// Cassandra Gateway List Provider.
/// </summary>
internal sealed class CassandraGatewayListProvider : SessionContextProviderBase, IGatewayListProvider
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CassandraGatewayListProvider"/> class.
    /// </summary>
    /// <param name="name">The name (of client).</param>
    /// <param name="logger">The logger.</param>
    /// <param name="cluster">The cluster (client).</param>
    /// <param name="clientOptions">The client options.</param>
    /// <param name="clusterOptions">The cluster options.</param>
    /// <param name="options">The options.</param>
    public CassandraGatewayListProvider(
        string name,
        ILogger<CassandraGatewayListProvider> logger,
        ICluster cluster,
        CassandraClientOptions clientOptions,
        ClusterOptions clusterOptions,
        CassandraGatewayListProviderOptions options)
        : base(name, logger, cluster, clientOptions, clusterOptions)
    {
        ArgumentNullException.ThrowIfNull(options);
        MaxStaleness = options.MaxStaleness;
    }

    /// <summary>
    /// Finalizes an instance of the <see cref="CassandraGatewayListProvider"/> class.
    /// </summary>
    ~CassandraGatewayListProvider() => Dispose(false);

    /// <inheritdoc/>
    public TimeSpan MaxStaleness { get; }

    /// <inheritdoc/>
    public bool IsUpdatable => true;

    /// <inheritdoc/>
    public Task InitializeGatewayListProvider()
        => Initialize();

    /// <inheritdoc/>
    public Task<IList<Uri>> GetGateways()
        => GetGatewayList();
}
