// Copyright (c) Escendit Ltd. All Rights Reserved.
// Licensed under the MIT. See LICENSE.txt file in the solution root for full license information.

namespace Orleans.Hosting;

using Escendit.Extensions.Hosting.Cassandra;
using Escendit.Orleans.Clustering.Cassandra.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

/// <summary>
/// Clustering Client Builder Extensions.
/// </summary>
public static class ClusteringClientBuilderExtensions
{
    /// <summary>
    /// With Client As Default.
    /// </summary>
    /// <param name="clusteringClientBuilder">The clustering silo builder.</param>
    /// <param name="configureOptions">The configure options.</param>
    /// <returns>The updated clustering silo builder.</returns>
    public static IClusteringClientBuilder WithClientAsDefault(
        this IClusteringClientBuilder clusteringClientBuilder,
        Action<CassandraClientOptions> configureOptions)
    {
        ArgumentNullException.ThrowIfNull(clusteringClientBuilder);
        ArgumentNullException.ThrowIfNull(configureOptions);
        clusteringClientBuilder
            .Services
            .AddCassandraClientAsDefault(configureOptions)
            .Configure<CassandraClusteringOptions>(options => options.ClientName = CassandraClientOptions.DefaultOptionsKey);
        return clusteringClientBuilder;
    }

    /// <summary>
    /// Add Client As Default.
    /// </summary>
    /// <param name="clusteringClientBuilder">The clustering silo builder.</param>
    /// <param name="configureOptions">The configure options.</param>
    /// <returns>The updated clustering silo builder.</returns>
    public static IClusteringClientBuilder WithClientAsDefault(
        this IClusteringClientBuilder clusteringClientBuilder,
        Action<OptionsBuilder<CassandraClientOptions>> configureOptions)
    {
        ArgumentNullException.ThrowIfNull(clusteringClientBuilder);
        ArgumentNullException.ThrowIfNull(configureOptions);
        clusteringClientBuilder
            .Services
            .AddCassandraClientAsDefault(configureOptions)
            .Configure<CassandraClusteringOptions>(options => options.ClientName = CassandraClientOptions.DefaultOptionsKey);
        return clusteringClientBuilder;
    }

    /// <summary>
    /// Add Client As Default.
    /// </summary>
    /// <param name="clusteringClientBuilder">The clustering silo builder.</param>
    /// <param name="optionsName">The options name.</param>
    /// <returns>The updated clustering silo builder.</returns>
    public static IClusteringClientBuilder WithClientFromOptionsAsDefault(
        this IClusteringClientBuilder clusteringClientBuilder,
        string optionsName)
    {
        ArgumentNullException.ThrowIfNull(clusteringClientBuilder);
        ArgumentNullException.ThrowIfNull(optionsName);
        clusteringClientBuilder
            .Services
            .AddCassandraClientFromOptionsAsDefault(optionsName)
            .Configure<CassandraClusteringOptions>(options => options.ClientName = CassandraClientOptions.DefaultOptionsKey);
        return clusteringClientBuilder;
    }

    /// <summary>
    /// Add Client.
    /// </summary>
    /// <param name="clusteringClientBuilder">The initial clustering silo builder.</param>
    /// <param name="name">The name.</param>
    /// <param name="configureOptions">The configure options.</param>
    /// <returns>The updated clustering silo builder.</returns>
    public static IClusteringClientBuilder WithClient(
        this IClusteringClientBuilder clusteringClientBuilder,
        string name,
        Action<CassandraClientOptions> configureOptions)
    {
        ArgumentNullException.ThrowIfNull(clusteringClientBuilder);
        ArgumentNullException.ThrowIfNull(name);
        ArgumentNullException.ThrowIfNull(configureOptions);
        clusteringClientBuilder
            .Services
            .AddCassandraClient(name, configureOptions)
            .Configure<CassandraClusteringOptions>(options => options.ClientName = name);
        return clusteringClientBuilder;
    }

    /// <summary>
    /// Add Client.
    /// </summary>
    /// <param name="clusteringClientBuilder">The initial clustering silo builder.</param>
    /// <param name="name">The name.</param>
    /// <param name="configureOptions">The configure options.</param>
    /// <returns>The updated clustering silo builder.</returns>
    public static IClusteringClientBuilder WithClient(
        this IClusteringClientBuilder clusteringClientBuilder,
        string name,
        Action<OptionsBuilder<CassandraClientOptions>> configureOptions)
    {
        ArgumentNullException.ThrowIfNull(clusteringClientBuilder);
        ArgumentNullException.ThrowIfNull(name);
        ArgumentNullException.ThrowIfNull(configureOptions);
        clusteringClientBuilder.Configure<CassandraClusteringOptions>(options => options.ClientName = name);
        clusteringClientBuilder
            .Services
            .AddCassandraClient(name, configureOptions)
            .Configure<CassandraClusteringOptions>(options => options.ClientName = name);
        return clusteringClientBuilder;
    }

    /// <summary>
    /// Add Client.
    /// </summary>
    /// <param name="clusteringClientBuilder">The initial clustering silo builder.</param>
    /// <param name="name">The name.</param>
    /// <param name="optionsName">The options name.</param>
    /// <returns>The updated clustering silo builder.</returns>
    public static IClusteringClientBuilder WithClientFromOptions(
        this IClusteringClientBuilder clusteringClientBuilder,
        string name,
        string optionsName)
    {
        ArgumentNullException.ThrowIfNull(clusteringClientBuilder);
        ArgumentNullException.ThrowIfNull(name);
        ArgumentNullException.ThrowIfNull(optionsName);
        clusteringClientBuilder
            .Services
            .AddCassandraClientFromOptions(name, optionsName)
            .Configure<CassandraClusteringOptions>(options => options.ClientName = name);
        return clusteringClientBuilder;
    }

    /// <summary>
    /// With Options.
    /// </summary>
    /// <param name="clusteringClientBuilder">The clustering silo builder.</param>
    /// <param name="configureOptions">The configure options.</param>
    /// <returns>The updated clustering silo builder.</returns>
    public static IClusteringClientBuilder WithOptions(
        this IClusteringClientBuilder clusteringClientBuilder,
        Action<CassandraClusteringOptions> configureOptions)
    {
        ArgumentNullException.ThrowIfNull(clusteringClientBuilder);
        ArgumentNullException.ThrowIfNull(configureOptions);
        return clusteringClientBuilder
            .WithOptions(builder =>
                builder.Configure(configureOptions));
    }

    /// <summary>
    /// With Options.
    /// </summary>
    /// <param name="clusteringClientBuilder">The initial clustering silo builder.</param>
    /// <param name="configureOptions">The configure options.</param>
    /// <returns>The updated clustering silo builder.</returns>
    public static IClusteringClientBuilder WithOptions(
        this IClusteringClientBuilder clusteringClientBuilder,
        Action<OptionsBuilder<CassandraClusteringOptions>> configureOptions)
    {
        ArgumentNullException.ThrowIfNull(clusteringClientBuilder);
        ArgumentNullException.ThrowIfNull(configureOptions);
        configureOptions.Invoke(clusteringClientBuilder.Services.AddOptions<CassandraClusteringOptions>());
        return clusteringClientBuilder;
    }
}
