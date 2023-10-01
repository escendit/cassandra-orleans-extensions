// Copyright (c) Escendit Ltd. All Rights Reserved.
// Licensed under the MIT. See LICENSE.txt file in the solution root for full license information.

namespace Orleans.Hosting;

using Cassandra;
using Escendit.Extensions.Hosting.Cassandra;
using Escendit.Orleans.Clustering.Cassandra;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Runtime;

/// <summary>
/// Clustering Client Builder Extensions.
/// </summary>
public static class ClientClusteringBuilderExtensions
{
    /// <summary>
    /// With Client As Default.
    /// </summary>
    /// <param name="clientClusteringBuilder">The clustering silo builder.</param>
    /// <param name="configureOptions">The configure options.</param>
    /// <returns>The updated clustering silo builder.</returns>
    public static IClientClusteringBuilder WithClientAsDefault(
        this IClientClusteringBuilder clientClusteringBuilder,
        Action<CassandraClientOptions> configureOptions)
    {
        ArgumentNullException.ThrowIfNull(clientClusteringBuilder);
        ArgumentNullException.ThrowIfNull(configureOptions);
        clientClusteringBuilder.Name = CassandraClientOptions.DefaultOptionsKey;
        clientClusteringBuilder
            .Services
            .AddCassandraClientAsDefault(configureOptions);
        return clientClusteringBuilder;
    }

    /// <summary>
    /// Add Client As Default.
    /// </summary>
    /// <param name="clientClusteringBuilder">The clustering silo builder.</param>
    /// <param name="configureOptions">The configure options.</param>
    /// <returns>The updated clustering silo builder.</returns>
    public static IClientClusteringBuilder WithClientAsDefault(
        this IClientClusteringBuilder clientClusteringBuilder,
        Action<OptionsBuilder<CassandraClientOptions>> configureOptions)
    {
        ArgumentNullException.ThrowIfNull(clientClusteringBuilder);
        ArgumentNullException.ThrowIfNull(configureOptions);
        clientClusteringBuilder.Name = CassandraClientOptions.DefaultOptionsKey;
        clientClusteringBuilder
            .Services
            .AddCassandraClientAsDefault(configureOptions);
        return clientClusteringBuilder;
    }

    /// <summary>
    /// Add Client As Default.
    /// </summary>
    /// <param name="clientClusteringBuilder">The clustering silo builder.</param>
    /// <param name="optionsName">The options name.</param>
    /// <returns>The updated clustering silo builder.</returns>
    public static IClientClusteringBuilder WithClientFromOptionsAsDefault(
        this IClientClusteringBuilder clientClusteringBuilder,
        string optionsName)
    {
        ArgumentNullException.ThrowIfNull(clientClusteringBuilder);
        ArgumentNullException.ThrowIfNull(optionsName);
        clientClusteringBuilder.Name = CassandraClientOptions.DefaultOptionsKey;
        clientClusteringBuilder
            .Services
            .AddCassandraClientFromOptionsAsDefault(optionsName);
        return clientClusteringBuilder;
    }

    /// <summary>
    /// Add Client.
    /// </summary>
    /// <param name="clientClusteringBuilder">The initial clustering silo builder.</param>
    /// <param name="name">The name.</param>
    /// <param name="configureOptions">The configure options.</param>
    /// <returns>The updated clustering silo builder.</returns>
    public static IClientClusteringBuilder WithClient(
        this IClientClusteringBuilder clientClusteringBuilder,
        string name,
        Action<CassandraClientOptions> configureOptions)
    {
        ArgumentNullException.ThrowIfNull(clientClusteringBuilder);
        ArgumentNullException.ThrowIfNull(name);
        ArgumentNullException.ThrowIfNull(configureOptions);
        clientClusteringBuilder.Name = CassandraClientOptions.DefaultOptionsKey;
        clientClusteringBuilder
            .Services
            .AddCassandraClient(name, configureOptions);
        return clientClusteringBuilder;
    }

    /// <summary>
    /// Add Client.
    /// </summary>
    /// <param name="clientClusteringBuilder">The initial clustering silo builder.</param>
    /// <param name="name">The name.</param>
    /// <param name="configureOptions">The configure options.</param>
    /// <returns>The updated clustering silo builder.</returns>
    public static IClientClusteringBuilder WithClient(
        this IClientClusteringBuilder clientClusteringBuilder,
        string name,
        Action<OptionsBuilder<CassandraClientOptions>> configureOptions)
    {
        ArgumentNullException.ThrowIfNull(clientClusteringBuilder);
        ArgumentNullException.ThrowIfNull(name);
        ArgumentNullException.ThrowIfNull(configureOptions);
        clientClusteringBuilder.Name = name;
        clientClusteringBuilder
            .Services
            .AddCassandraClient(name, configureOptions);
        return clientClusteringBuilder;
    }

    /// <summary>
    /// Add Client.
    /// </summary>
    /// <param name="clientClusteringBuilder">The initial clustering silo builder.</param>
    /// <param name="name">The name.</param>
    /// <param name="optionsName">The options name.</param>
    /// <returns>The updated clustering silo builder.</returns>
    public static IClientClusteringBuilder WithClientFromOptions(
        this IClientClusteringBuilder clientClusteringBuilder,
        string name,
        string optionsName)
    {
        ArgumentNullException.ThrowIfNull(clientClusteringBuilder);
        ArgumentNullException.ThrowIfNull(name);
        ArgumentNullException.ThrowIfNull(optionsName);
        clientClusteringBuilder.Name = name;
        clientClusteringBuilder
            .Services
            .AddCassandraClientFromOptions(name, optionsName);
        return clientClusteringBuilder;
    }

    /// <summary>
    /// Build.
    /// </summary>
    /// <param name="clientClusteringBuilder">The clustering silo builder.</param>
    /// <returns>The silo builder.</returns>
    public static IClientBuilder Build(this IClientClusteringBuilder clientClusteringBuilder)
    {
        ArgumentNullException.ThrowIfNull(clientClusteringBuilder);
        clientClusteringBuilder
            .Services
            .AddSingleton<IMembershipTable>(serviceProvider =>
                ActivatorUtilities.CreateInstance<CassandraMembershipTable>(
                    serviceProvider,
                    clientClusteringBuilder.Name,
                    serviceProvider.GetRequiredServiceByName<ICluster>(clientClusteringBuilder.Name)));
        return clientClusteringBuilder;
    }
}
