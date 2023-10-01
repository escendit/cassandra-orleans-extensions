// Copyright (c) Escendit Ltd. All Rights Reserved.
// Licensed under the MIT. See LICENSE.txt file in the solution root for full license information.

namespace Orleans.Hosting;

using Escendit.Extensions.Hosting.Cassandra;
using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Clustering Client Builder.
/// </summary>
internal class ClientClusteringBuilder : IClientClusteringBuilder
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ClientClusteringBuilder"/> class.
    /// </summary>
    /// <param name="serviceCollection">The service collection.</param>
    public ClientClusteringBuilder(IServiceCollection serviceCollection)
    {
        Services = serviceCollection;
    }

    /// <inheritdoc/>
    public IServiceCollection Services { get; }

    /// <inheritdoc/>
    public string Name { get; set; } = CassandraClientOptions.DefaultOptionsKey;
}
