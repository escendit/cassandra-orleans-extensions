// Copyright (c) Escendit Ltd. All Rights Reserved.
// Licensed under the MIT. See LICENSE.txt file in the solution root for full license information.

namespace Escendit.Orleans.Persistence.Cassandra.Hosting;

using Escendit.Extensions.Hosting.Cassandra;
using global::Orleans.Hosting;
using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Silo Persistence Builder Extensions.
/// </summary>
public static class SiloPersistenceBuilderExtensions
{
    /// <summary>
    /// Add Client.
    /// </summary>
    /// <param name="siloPersistenceBuilder">The initial silo persistence builder.</param>
    /// <param name="configureOptions">The configure options.</param>
    /// <returns>The updated silo persistence builder.</returns>
    public static ISiloPersistenceBuilder UseClient(
        this ISiloPersistenceBuilder siloPersistenceBuilder,
        Action<CassandraClientOptions> configureOptions)
    {
        ArgumentNullException.ThrowIfNull(siloPersistenceBuilder);
        ArgumentNullException.ThrowIfNull(siloPersistenceBuilder.Name);
        ArgumentNullException.ThrowIfNull(2);
        siloPersistenceBuilder
            .ConfigureServices(services => services
                .AddCassandraClient(siloPersistenceBuilder.Name, configureOptions));
        return siloPersistenceBuilder;
    }
}
