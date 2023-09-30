// Copyright (c) Escendit Ltd. All Rights Reserved.
// Licensed under the MIT. See LICENSE.txt file in the solution root for full license information.

namespace Orleans.Hosting;

using Escendit.Extensions.Hosting.Cassandra;
using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Clustering Silo Builder.
/// </summary>
internal class SiloClusteringBuilder : ISiloClusteringBuilder
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SiloClusteringBuilder"/> class.
    /// </summary>
    /// <param name="serviceCollection">The service collection.</param>
    public SiloClusteringBuilder(IServiceCollection serviceCollection)
    {
        Services = serviceCollection;
    }

    /// <inheritdoc/>
    public IServiceCollection Services { get; }

    /// <inheritdoc/>
    public string Name { get; set; } = CassandraClientOptions.DefaultOptionsKey;
}
