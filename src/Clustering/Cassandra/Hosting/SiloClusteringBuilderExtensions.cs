// Copyright (c) Escendit Ltd. All Rights Reserved.
// Licensed under the MIT. See LICENSE.txt file in the solution root for full license information.

namespace Orleans.Hosting;

using Cassandra;
using Configuration.Overrides;
using Escendit.Extensions.Hosting.Cassandra;
using Escendit.Orleans.Clustering.Cassandra;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Runtime;

/// <summary>
/// Clustering Silo Builder Extensions.
/// </summary>
public static class SiloClusteringBuilderExtensions
{
    /// <summary>
    /// With Client As Default.
    /// </summary>
    /// <param name="siloClusteringBuilder">The clustering silo builder.</param>
    /// <param name="configureOptions">The configure options.</param>
    /// <returns>The updated clustering silo builder.</returns>
    public static ISiloClusteringBuilder WithClientAsDefault(
        this ISiloClusteringBuilder siloClusteringBuilder,
        Action<CassandraClientOptions> configureOptions)
    {
        ArgumentNullException.ThrowIfNull(siloClusteringBuilder);
        ArgumentNullException.ThrowIfNull(configureOptions);
        siloClusteringBuilder.Name = CassandraClientOptions.DefaultOptionsKey;
        siloClusteringBuilder
            .Services
            .AddCassandraClientAsDefault(configureOptions);
        return siloClusteringBuilder;
    }

    /// <summary>
    /// Add Client As Default.
    /// </summary>
    /// <param name="siloClusteringBuilder">The clustering silo builder.</param>
    /// <param name="configureOptions">The configure options.</param>
    /// <returns>The updated clustering silo builder.</returns>
    public static ISiloClusteringBuilder WithClientAsDefault(
        this ISiloClusteringBuilder siloClusteringBuilder,
        Action<OptionsBuilder<CassandraClientOptions>> configureOptions)
    {
        ArgumentNullException.ThrowIfNull(siloClusteringBuilder);
        ArgumentNullException.ThrowIfNull(configureOptions);
        siloClusteringBuilder.Name = CassandraClientOptions.DefaultOptionsKey;
        siloClusteringBuilder
            .Services
            .AddCassandraClientAsDefault(configureOptions);
        return siloClusteringBuilder;
    }

    /// <summary>
    /// Add Client As Default.
    /// </summary>
    /// <param name="siloClusteringBuilder">The clustering silo builder.</param>
    /// <param name="optionsName">The options name.</param>
    /// <returns>The updated clustering silo builder.</returns>
    public static ISiloClusteringBuilder WithClientFromOptionsAsDefault(
        this ISiloClusteringBuilder siloClusteringBuilder,
        string optionsName)
    {
        ArgumentNullException.ThrowIfNull(siloClusteringBuilder);
        ArgumentNullException.ThrowIfNull(optionsName);
        siloClusteringBuilder.Name = CassandraClientOptions.DefaultOptionsKey;
        siloClusteringBuilder
            .Services
            .AddCassandraClientFromOptionsAsDefault(optionsName);
        return siloClusteringBuilder;
    }

    /// <summary>
    /// Add Client.
    /// </summary>
    /// <param name="siloClusteringBuilder">The initial clustering silo builder.</param>
    /// <param name="name">The name.</param>
    /// <param name="configureOptions">The configure options.</param>
    /// <returns>The updated clustering silo builder.</returns>
    public static ISiloClusteringBuilder WithClient(
        this ISiloClusteringBuilder siloClusteringBuilder,
        string name,
        Action<CassandraClientOptions> configureOptions)
    {
        ArgumentNullException.ThrowIfNull(siloClusteringBuilder);
        ArgumentNullException.ThrowIfNull(name);
        ArgumentNullException.ThrowIfNull(configureOptions);
        siloClusteringBuilder.Name = name;
        siloClusteringBuilder
            .Services
            .AddCassandraClient(name, configureOptions);
        return siloClusteringBuilder;
    }

    /// <summary>
    /// Add Client.
    /// </summary>
    /// <param name="siloClusteringBuilder">The initial clustering silo builder.</param>
    /// <param name="name">The name.</param>
    /// <param name="configureOptions">The configure options.</param>
    /// <returns>The updated clustering silo builder.</returns>
    public static ISiloClusteringBuilder WithClient(
        this ISiloClusteringBuilder siloClusteringBuilder,
        string name,
        Action<OptionsBuilder<CassandraClientOptions>> configureOptions)
    {
        ArgumentNullException.ThrowIfNull(siloClusteringBuilder);
        ArgumentNullException.ThrowIfNull(name);
        ArgumentNullException.ThrowIfNull(configureOptions);
        siloClusteringBuilder.Name = name;
        siloClusteringBuilder
            .Services
            .AddCassandraClient(name, configureOptions);
        return siloClusteringBuilder;
    }

    /// <summary>
    /// Add Client.
    /// </summary>
    /// <param name="siloClusteringBuilder">The initial clustering silo builder.</param>
    /// <param name="name">The name.</param>
    /// <param name="optionsName">The options name.</param>
    /// <returns>The updated clustering silo builder.</returns>
    public static ISiloClusteringBuilder WithClientFromOptions(
        this ISiloClusteringBuilder siloClusteringBuilder,
        string name,
        string optionsName)
    {
        ArgumentNullException.ThrowIfNull(siloClusteringBuilder);
        ArgumentNullException.ThrowIfNull(name);
        ArgumentNullException.ThrowIfNull(optionsName);
        siloClusteringBuilder.Name = name;
        siloClusteringBuilder
            .Services
            .AddCassandraClientFromOptions(name, optionsName);
        return siloClusteringBuilder;
    }

    /// <summary>
    /// Build.
    /// </summary>
    /// <param name="siloClusteringBuilder">The silo clustering builder.</param>
    /// <returns>The updated silo builder.</returns>
    public static ISiloBuilder Build(
        this ISiloClusteringBuilder siloClusteringBuilder)
    {
        ArgumentNullException.ThrowIfNull(siloClusteringBuilder);
        siloClusteringBuilder
            .Services
            .AddSingleton<IMembershipTable>(serviceProvider =>
                ActivatorUtilities.CreateInstance<CassandraMembershipTable>(
                    serviceProvider,
                    siloClusteringBuilder.Name,
                    serviceProvider.GetRequiredServiceByName<ICluster>(siloClusteringBuilder.Name),
                    serviceProvider.GetOptionsByName<CassandraClientOptions>(siloClusteringBuilder.Name),
                    serviceProvider.GetProviderClusterOptions(siloClusteringBuilder.Name).Value));
        return siloClusteringBuilder;
    }
}
