// Copyright (c) Escendit Ltd. All Rights Reserved.
// Licensed under the MIT. See LICENSE.txt file in the solution root for full license information.

namespace Orleans.Hosting;

using Cassandra;
using Escendit.Extensions.Hosting.Cassandra;
using Escendit.Orleans.Reminders.Cassandra.Provider;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Runtime;

/// <summary>
/// Silo Builder Extensions.
/// </summary>
public static class SiloBuilderExtensions
{
    /// <summary>
    /// Use Cassandra Reminder Service.
    /// </summary>
    /// <param name="siloBuilder">The initial silo builder.</param>
    /// <param name="configureOptions">The configure options.</param>
    /// <returns>The updated silo builder.</returns>
    public static ISiloBuilder UseCassandraReminderService(
        this ISiloBuilder siloBuilder,
        Action<CassandraClientOptions> configureOptions)
    {
        return siloBuilder
            .UseCassandraReminderService(builder => builder
                .Configure(configureOptions));
    }

    /// <summary>
    /// Use Cassandra Reminder Service.
    /// </summary>
    /// <param name="siloBuilder">The initial silo builder.</param>
    /// <param name="configureOptions">The configure options.</param>
    /// <returns>The updated silo builder.</returns>
    public static ISiloBuilder UseCassandraReminderService(
        this ISiloBuilder siloBuilder,
        Action<OptionsBuilder<CassandraClientOptions>> configureOptions)
    {
        ArgumentNullException.ThrowIfNull(siloBuilder);
        ArgumentNullException.ThrowIfNull(configureOptions);
        siloBuilder
            .Services
            .AddCassandraClientAsDefault(configureOptions)
            .AddSingleton<IReminderTable>(serviceProvider =>
                Create(serviceProvider, CassandraClientOptions.DefaultOptionsKey))
            .AddReminders();
        return siloBuilder;
    }

    /// <summary>
    /// Use Cassandra Reminder Service.
    /// </summary>
    /// <param name="siloBuilder">The initial silo builder.</param>
    /// <param name="name">The name (of client).</param>
    /// <param name="configureOptions">The configure options.</param>
    /// <returns>The updated silo builder.</returns>
    public static ISiloBuilder UseCassandraReminderService(
        this ISiloBuilder siloBuilder,
        string name,
        Action<CassandraClientOptions> configureOptions)
    {
        return siloBuilder
            .UseCassandraReminderService(name, builder => builder
                .Configure(configureOptions));
    }

    /// <summary>
    /// Use Cassandra Reminder Service.
    /// </summary>
    /// <param name="siloBuilder">The initial silo builder.</param>
    /// <param name="name">The name (of client).</param>
    /// <param name="configureOptions">The configure options.</param>
    /// <returns>The updated silo builder.</returns>
    public static ISiloBuilder UseCassandraReminderService(
        this ISiloBuilder siloBuilder,
        string name,
        Action<OptionsBuilder<CassandraClientOptions>> configureOptions)
    {
        ArgumentNullException.ThrowIfNull(siloBuilder);
        ArgumentNullException.ThrowIfNull(configureOptions);
        siloBuilder
            .Services
            .AddCassandraClient(name, configureOptions)
            .AddSingleton<IReminderTable>(serviceProvider => Create(serviceProvider, name))
            .AddReminders();
        return siloBuilder;
    }

    /// <summary>
    /// Use Cassandra Reminder Service.
    /// </summary>
    /// <para>
    /// Use if we already created the client.
    /// </para>
    /// <param name="siloBuilder">The initial silo builder.</param>
    /// <param name="clientName">The client name.</param>
    /// <returns>The updated silo builder.</returns>
    public static ISiloBuilder UseCassandraReminderService(
        this ISiloBuilder siloBuilder,
        string clientName)
    {
        ArgumentNullException.ThrowIfNull(siloBuilder);
        ArgumentNullException.ThrowIfNull(clientName);
        siloBuilder
            .Services
            .AddSingleton<IReminderTable>(serviceProvider => Create(serviceProvider, clientName))
            .AddReminders();
        return siloBuilder;
    }

    private static CassandraRemindersTable Create(IServiceProvider serviceProvider, string name)
        => ActivatorUtilities
            .CreateInstance<CassandraRemindersTable>(
                serviceProvider,
                name,
                serviceProvider.GetRequiredServiceByName<ICluster>(name),
                serviceProvider.GetOptionsByName<CassandraClientOptions>(name));
}
