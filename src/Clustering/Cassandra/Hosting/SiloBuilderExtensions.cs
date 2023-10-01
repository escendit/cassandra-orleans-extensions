// Copyright (c) Escendit Ltd. All Rights Reserved.
// Licensed under the MIT. See LICENSE.txt file in the solution root for full license information.

namespace Orleans.Hosting;

using Escendit.Orleans.Clustering.Cassandra;
using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Silo Builder Extensions.
/// </summary>
public static class SiloBuilderExtensions
{
    /// <summary>
    /// Use Cassandra Clustering.
    /// </summary>
    /// <param name="siloBuilder">The initial silo builder.</param>
    /// <returns>The updated silo builder.</returns>
    public static ISiloClusteringBuilder UseCassandraClustering(
        this ISiloBuilder siloBuilder)
    {
        ArgumentNullException.ThrowIfNull(siloBuilder);
        return new SiloClusteringBuilder(siloBuilder.Services);
    }
}
