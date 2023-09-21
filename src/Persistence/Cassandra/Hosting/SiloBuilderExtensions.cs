// Copyright (c) Escendit Ltd. All Rights Reserved.
// Licensed under the MIT. See LICENSE.txt file in the solution root for full license information.

namespace Orleans.Hosting;

using Escendit.Orleans.Persistence.Cassandra.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Providers;

/// <summary>
/// Silo Builder Extensions.
/// </summary>
public static class SiloBuilderExtensions
{
/// <summary>
    /// Add Cassandra Grain Storage As Default.
    /// </summary>
    /// <param name="services">The initial service collection.</param>
    /// <param name="configureOptions">The configure options.</param>
    /// <returns>The updated service collection.</returns>
    public static ISiloBuilder AddCassandraGrainStorageAsDefault(
        this ISiloBuilder services,
        Action<CassandraStorageOptions> configureOptions)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configureOptions);
        return services
            .AddCassandraGrainStorage(
                ProviderConstants.DEFAULT_STORAGE_PROVIDER_NAME,
                configureOptions);
    }

    /// <summary>
    /// Add Cassandra Grain Storage As Default.
    /// </summary>
    /// <param name="services">The initial service collection.</param>
    /// <param name="configureOptions">The configure options.</param>
    /// <returns>The updated service collection.</returns>
    public static ISiloBuilder AddCassandraGrainStorageAsDefault(
        this ISiloBuilder services,
        Action<OptionsBuilder<CassandraStorageOptions>> configureOptions)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configureOptions);
        return services
            .AddCassandraGrainStorage(
                ProviderConstants.DEFAULT_STORAGE_PROVIDER_NAME,
                configureOptions);
    }

    /// <summary>
    /// Add Cassandra Grain Storage.
    /// </summary>
    /// <param name="siloBuilder">The initial service collection.</param>
    /// <param name="name">The name.</param>
    /// <param name="configureOptions">The configure options.</param>
    /// <returns>The updated service collection.</returns>
    public static ISiloBuilder AddCassandraGrainStorage(
        this ISiloBuilder siloBuilder,
        string name,
        Action<CassandraStorageOptions> configureOptions)
    {
        ArgumentNullException.ThrowIfNull(siloBuilder);
        ArgumentNullException.ThrowIfNull(name);
        ArgumentNullException.ThrowIfNull(configureOptions);
        return siloBuilder
            .ConfigureServices(services => services
                .AddCassandraGrainStorage(name, configureOptions));
    }

    /// <summary>
    /// Add Cassandra Grain Storage.
    /// </summary>
    /// <param name="siloBuilder">The silo builder.</param>
    /// <param name="name">The name.</param>
    /// <param name="configureOptions">The configure options.</param>
    /// <returns>The updated service collection.</returns>
    public static ISiloBuilder AddCassandraGrainStorage(
        this ISiloBuilder siloBuilder,
        string name,
        Action<OptionsBuilder<CassandraStorageOptions>> configureOptions)
    {
        ArgumentNullException.ThrowIfNull(siloBuilder);
        ArgumentNullException.ThrowIfNull(name);
        ArgumentNullException.ThrowIfNull(configureOptions);
        return siloBuilder
            .ConfigureServices(services => services
                .AddCassandraGrainStorage(name, configureOptions));
    }
}
