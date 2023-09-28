// Copyright (c) Escendit Ltd. All Rights Reserved.
// Licensed under the MIT. See LICENSE.txt file in the solution root for full license information.

namespace Microsoft.Extensions.DependencyInjection;

using Escendit.Orleans.Clustering.Cassandra;
using Escendit.Orleans.Clustering.Cassandra.Options;
using Options;
using Orleans;
using Orleans.Messaging;

/// <summary>
/// Service Collection Extensions.
/// </summary>
internal static class ServiceCollectionExtensions
{
    /// <summary>
    /// Add Cassandra Clustering.
    /// </summary>
    /// <param name="services">The initial service collection.</param>
    /// <param name="configureOptions">The configure options.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddCassandraClustering(
        this IServiceCollection services,
        Action<CassandraClusteringOptions> configureOptions)
    {
        return services
            .AddCassandraClustering(builder => builder.Configure(configureOptions));
    }

    /// <summary>
    /// Add Cassandra Clustering.
    /// </summary>
    /// <param name="services">The initial service collection.</param>
    /// <param name="configureOptions">The configure options.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddCassandraClustering(
        this IServiceCollection services,
        Action<OptionsBuilder<CassandraClusteringOptions>> configureOptions)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configureOptions);
        configureOptions.Invoke(services.AddOptions<CassandraClusteringOptions>());
        return services
            .AddSingleton<IGatewayListProvider, CassandraGatewayListProvider>()
            .AddSingleton<IMembershipTable, CassandraMembershipTable>();
    }
}
