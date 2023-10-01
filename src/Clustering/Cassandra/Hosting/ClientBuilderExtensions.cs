// Copyright (c) Escendit Ltd. All Rights Reserved.
// Licensed under the MIT. See LICENSE.txt file in the solution root for full license information.

namespace Orleans.Hosting;

using Escendit.Orleans.Clustering.Cassandra.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

/// <summary>
/// Client Builder Extensions.
/// </summary>
public static class ClientBuilderExtensions
{
    /// <summary>
    /// Use Cassandra Clustering.
    /// </summary>
    /// <param name="clientBuilder">The initial silo builder.</param>
    /// <param name="configureOptions">The configure options.</param>
    /// <returns>The updated silo builder.</returns>
    public static IClientClusteringBuilder UseCassandraClustering(
        this IClientBuilder clientBuilder,
        Action<CassandraGatewayListProviderOptions> configureOptions)
    {
        ArgumentNullException.ThrowIfNull(clientBuilder);
        clientBuilder
            .UseCassandraClustering(
                builder => builder.Configure(configureOptions));
        return new ClientClusteringBuilder(clientBuilder.Services);
    }

    /// <summary>
    /// Use Cassandra Clustering.
    /// </summary>
    /// <param name="clientBuilder">The initial silo builder.</param>
    /// <param name="configureOptions">The configure options.</param>
    /// <returns>The updated silo builder.</returns>
    public static IClientClusteringBuilder UseCassandraClustering(
        this IClientBuilder clientBuilder,
        Action<OptionsBuilder<CassandraGatewayListProviderOptions>> configureOptions)
    {
        ArgumentNullException.ThrowIfNull(clientBuilder);
        configureOptions.Invoke(clientBuilder.Services.AddOptions<CassandraGatewayListProviderOptions>());
        return new ClientClusteringBuilder(clientBuilder.Services);
    }

    /// <summary>
    /// Use Cassandra Clustering.
    /// </summary>
    /// <param name="clientBuilder">The initial silo builder.</param>
    /// <returns>The updated silo builder.</returns>
    public static IClientClusteringBuilder UseCassandraClustering(this IClientBuilder clientBuilder)
    {
        ArgumentNullException.ThrowIfNull(clientBuilder);
        clientBuilder.Services.AddOptions<CassandraGatewayListProviderOptions>();
        return new ClientClusteringBuilder(clientBuilder.Services);
    }
}
