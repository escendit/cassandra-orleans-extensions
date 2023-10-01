// Copyright (c) Escendit Ltd. All Rights Reserved.
// Licensed under the MIT. See LICENSE.txt file in the solution root for full license information.

namespace Microsoft.Extensions.DependencyInjection;

using Escendit.Orleans.Persistence.Cassandra.Hosting;
using Escendit.Orleans.Persistence.Cassandra.Options;
using Escendit.Orleans.Persistence.Cassandra.Storage;
using Extensions;
using Options;
using Orleans;
using Orleans.Configuration;
using Orleans.Providers;
using Orleans.Runtime;
using Orleans.Storage;

/// <summary>
/// Service Collection Extensions.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Add Cassandra Grain Storage As Default.
    /// </summary>
    /// <param name="services">The initial service collection.</param>
    /// <param name="configureOptions">The configure options.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddCassandraGrainStorageAsDefault(
        this IServiceCollection services,
        Action<CassandraStorageOptions> configureOptions)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configureOptions);
        return services
            .AddCassandraGrainStorage(
                ProviderConstants.DEFAULT_STORAGE_PROVIDER_NAME,
                builder => builder.Configure(configureOptions));
    }

    /// <summary>
    /// Add Cassandra Grain Storage As Default.
    /// </summary>
    /// <param name="services">The initial service collection.</param>
    /// <param name="configureOptions">The configure options.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddCassandraGrainStorageAsDefault(
        this IServiceCollection services,
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
    /// <param name="services">The initial service collection.</param>
    /// <param name="name">The name.</param>
    /// <param name="configureOptions">The configure options.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddCassandraGrainStorage(
        this IServiceCollection services,
        string name,
        Action<CassandraStorageOptions> configureOptions)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(name);
        ArgumentNullException.ThrowIfNull(configureOptions);
        return services
            .AddCassandraGrainStorage(name, builder => builder.Configure(configureOptions));
    }

    /// <summary>
    /// Add Cassandra Grain Storage.
    /// </summary>
    /// <param name="services">The initial service collection.</param>
    /// <param name="name">The name.</param>
    /// <param name="configureOptions">The configure options.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddCassandraGrainStorage(
        this IServiceCollection services,
        string name,
        Action<OptionsBuilder<CassandraStorageOptions>> configureOptions)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(name);
        ArgumentNullException.ThrowIfNull(configureOptions);
        configureOptions.Invoke(services.AddOptions<CassandraStorageOptions>(name));
        if (string.Equals(name, ProviderConstants.DEFAULT_STORAGE_PROVIDER_NAME, StringComparison.Ordinal))
        {
            services.TryAddSingleton<IGrainStorage>(serviceProvider =>
                serviceProvider.GetRequiredServiceByName<IGrainStorage>(ProviderConstants.DEFAULT_STORAGE_PROVIDER_NAME));
        }

        return services
            .AddTransient<IConfigurationValidator>(sp =>
                new CassandraStorageOptionsValidator(sp.GetOptionsByName<CassandraStorageOptions>(name), name))
            .ConfigureNamedOptionForLogging<CassandraStorageOptions>(name)
            .ConfigureOptions<DefaultCassandraPostConfigureOptions>()
            .AddSingletonNamedService<IGrainStorage>(name, CassandraGrainStorageFactory.Create)
            .AddSingletonNamedService<ILifecycleParticipant<ISiloLifecycle>>(name, (serviceProvider, providerName) =>
                (ILifecycleParticipant<ISiloLifecycle>)serviceProvider
                    .GetRequiredServiceByName<IGrainStorage>(providerName));
    }
}
