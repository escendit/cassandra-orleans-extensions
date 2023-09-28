// Copyright (c) Escendit Ltd. All Rights Reserved.
// Licensed under the MIT. See LICENSE.txt file in the solution root for full license information.

namespace Orleans.Hosting;

using Escendit.Orleans.Clustering.Cassandra;
using Messaging;
using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Client Builder Extensions.
/// </summary>
public static class ClientBuilderExtensions
{
    /// <summary>
    /// Use Cassandra Clustering.
    /// </summary>
    /// <param name="clientBuilder">The initial silo builder.</param>
    /// <returns>The updated silo builder.</returns>
    public static IClusteringClientBuilder UseCassandraClustering(this IClientBuilder clientBuilder)
    {
        ArgumentNullException.ThrowIfNull(clientBuilder);
        return new ClusteringClientBuilder(clientBuilder
            .Services
            .AddSingleton<IGatewayListProvider, CassandraGatewayListProvider>()
            .AddSingleton<IMembershipTable, CassandraMembershipTable>());
    }
}
