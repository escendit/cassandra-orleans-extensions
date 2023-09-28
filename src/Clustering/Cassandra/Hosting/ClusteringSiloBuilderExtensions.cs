// Copyright (c) Escendit Ltd. All Rights Reserved.
// Licensed under the MIT. See LICENSE.txt file in the solution root for full license information.

namespace Orleans.Hosting;

using Escendit.Extensions.Hosting.Cassandra;
using Escendit.Orleans.Clustering.Cassandra.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

/// <summary>
/// Clustering Silo Builder Extensions.
/// </summary>
public static class ClusteringSiloBuilderExtensions
{
    /// <summary>
    /// With Client As Default.
    /// </summary>
    /// <param name="clusteringSiloBuilder">The clustering silo builder.</param>
    /// <param name="configureOptions">The configure options.</param>
    /// <returns>The updated clustering silo builder.</returns>
    public static IClusteringSiloBuilder WithClientAsDefault(
        this IClusteringSiloBuilder clusteringSiloBuilder,
        Action<CassandraClientOptions> configureOptions)
    {
        ArgumentNullException.ThrowIfNull(clusteringSiloBuilder);
        ArgumentNullException.ThrowIfNull(configureOptions);
        clusteringSiloBuilder
            .Services
            .AddCassandraClientAsDefault(configureOptions)
            .Configure<CassandraClusteringOptions>(options => options.ClientName = CassandraClientOptions.DefaultOptionsKey);
        return clusteringSiloBuilder;
    }

    /// <summary>
    /// Add Client As Default.
    /// </summary>
    /// <param name="clusteringSiloBuilder">The clustering silo builder.</param>
    /// <param name="configureOptions">The configure options.</param>
    /// <returns>The updated clustering silo builder.</returns>
    public static IClusteringSiloBuilder WithClientAsDefault(
        this IClusteringSiloBuilder clusteringSiloBuilder,
        Action<OptionsBuilder<CassandraClientOptions>> configureOptions)
    {
        ArgumentNullException.ThrowIfNull(clusteringSiloBuilder);
        ArgumentNullException.ThrowIfNull(configureOptions);
        clusteringSiloBuilder
            .Services
            .AddCassandraClientAsDefault(configureOptions)
            .Configure<CassandraClusteringOptions>(options => options.ClientName = CassandraClientOptions.DefaultOptionsKey);
        return clusteringSiloBuilder;
    }

    /// <summary>
    /// Add Client As Default.
    /// </summary>
    /// <param name="clusteringSiloBuilder">The clustering silo builder.</param>
    /// <param name="optionsName">The options name.</param>
    /// <returns>The updated clustering silo builder.</returns>
    public static IClusteringSiloBuilder WithClientFromOptionsAsDefault(
        this IClusteringSiloBuilder clusteringSiloBuilder,
        string optionsName)
    {
        ArgumentNullException.ThrowIfNull(clusteringSiloBuilder);
        ArgumentNullException.ThrowIfNull(optionsName);
        clusteringSiloBuilder
            .Services
            .AddCassandraClientFromOptionsAsDefault(optionsName)
            .Configure<CassandraClusteringOptions>(options => options.ClientName = CassandraClientOptions.DefaultOptionsKey);
        return clusteringSiloBuilder;
    }

    /// <summary>
    /// Add Client.
    /// </summary>
    /// <param name="clusteringSiloBuilder">The initial clustering silo builder.</param>
    /// <param name="name">The name.</param>
    /// <param name="configureOptions">The configure options.</param>
    /// <returns>The updated clustering silo builder.</returns>
    public static IClusteringSiloBuilder WithClient(
        this IClusteringSiloBuilder clusteringSiloBuilder,
        string name,
        Action<CassandraClientOptions> configureOptions)
    {
        ArgumentNullException.ThrowIfNull(clusteringSiloBuilder);
        ArgumentNullException.ThrowIfNull(name);
        ArgumentNullException.ThrowIfNull(configureOptions);
        clusteringSiloBuilder
            .Services
            .AddCassandraClient(name, configureOptions)
            .Configure<CassandraClusteringOptions>(options => options.ClientName = name);
        return clusteringSiloBuilder;
    }

    /// <summary>
    /// Add Client.
    /// </summary>
    /// <param name="clusteringSiloBuilder">The initial clustering silo builder.</param>
    /// <param name="name">The name.</param>
    /// <param name="configureOptions">The configure options.</param>
    /// <returns>The updated clustering silo builder.</returns>
    public static IClusteringSiloBuilder WithClient(
        this IClusteringSiloBuilder clusteringSiloBuilder,
        string name,
        Action<OptionsBuilder<CassandraClientOptions>> configureOptions)
    {
        ArgumentNullException.ThrowIfNull(clusteringSiloBuilder);
        ArgumentNullException.ThrowIfNull(name);
        ArgumentNullException.ThrowIfNull(configureOptions);
        clusteringSiloBuilder.Configure<CassandraClusteringOptions>(options => options.ClientName = name);
        clusteringSiloBuilder
            .Services
            .AddCassandraClient(name, configureOptions)
            .Configure<CassandraClusteringOptions>(options => options.ClientName = name);
        return clusteringSiloBuilder;
    }

    /// <summary>
    /// Add Client.
    /// </summary>
    /// <param name="clusteringSiloBuilder">The initial clustering silo builder.</param>
    /// <param name="name">The name.</param>
    /// <param name="optionsName">The options name.</param>
    /// <returns>The updated clustering silo builder.</returns>
    public static IClusteringSiloBuilder WithClientFromOptions(
        this IClusteringSiloBuilder clusteringSiloBuilder,
        string name,
        string optionsName)
    {
        ArgumentNullException.ThrowIfNull(clusteringSiloBuilder);
        ArgumentNullException.ThrowIfNull(name);
        ArgumentNullException.ThrowIfNull(optionsName);
        clusteringSiloBuilder
            .Services
            .AddCassandraClientFromOptions(name, optionsName)
            .Configure<CassandraClusteringOptions>(options => options.ClientName = name);
        return clusteringSiloBuilder;
    }

    /// <summary>
    /// With Options.
    /// </summary>
    /// <param name="clusteringSiloBuilder">The clustering silo builder.</param>
    /// <param name="configureOptions">The configure options.</param>
    /// <returns>The updated clustering silo builder.</returns>
    public static IClusteringSiloBuilder WithOptions(
        this IClusteringSiloBuilder clusteringSiloBuilder,
        Action<CassandraClusteringOptions> configureOptions)
    {
        ArgumentNullException.ThrowIfNull(clusteringSiloBuilder);
        ArgumentNullException.ThrowIfNull(configureOptions);
        return clusteringSiloBuilder
            .WithOptions(builder =>
                builder.Configure(configureOptions));
    }

    /// <summary>
    /// With Options.
    /// </summary>
    /// <param name="clusteringSiloBuilder">The initial clustering silo builder.</param>
    /// <param name="configureOptions">The configure options.</param>
    /// <returns>The updated clustering silo builder.</returns>
    public static IClusteringSiloBuilder WithOptions(
        this IClusteringSiloBuilder clusteringSiloBuilder,
        Action<OptionsBuilder<CassandraClusteringOptions>> configureOptions)
    {
        ArgumentNullException.ThrowIfNull(clusteringSiloBuilder);
        ArgumentNullException.ThrowIfNull(configureOptions);
        configureOptions.Invoke(clusteringSiloBuilder.Services.AddOptions<CassandraClusteringOptions>());
        return clusteringSiloBuilder;
    }
}
