// Copyright (c) Escendit Ltd. All Rights Reserved.
// Licensed under the MIT. See LICENSE.txt file in the solution root for full license information.

namespace Escendit.Orleans.Persistence.Cassandra.Storage;

using Escendit.Extensions.Hosting.Cassandra;
using global::Orleans;
using Microsoft.Extensions.DependencyInjection;
using Options;

/// <summary>
/// Cassandra Grain Storage Factory.
/// </summary>
public static class CassandraGrainStorageFactory
{
    /// <summary>
    /// Create GrainStorage.
    /// </summary>
    /// <param name="serviceProvider">The service provider.</param>
    /// <param name="name">The name.</param>
    /// <returns>The grain storage.</returns>
    public static GrainStorageBase Create(IServiceProvider serviceProvider, string name)
    {
        var options = serviceProvider.GetOptionsByName<CassandraStorageOptions>(name);
        var connectionOptions = serviceProvider.GetOptionsByName<CassandraClientOptions>(name);
        return options.Strategy switch
        {
            Strategy.SingleTable => ActivatorUtilities
                .CreateInstance<SingleTableGrainStorage>(serviceProvider, name, options, connectionOptions),
            Strategy.TablePerGrain => throw new NotSupportedException(),
            null => throw new NotSupportedException(),
            _ => throw new ArgumentOutOfRangeException(name),
        };
    }
}
