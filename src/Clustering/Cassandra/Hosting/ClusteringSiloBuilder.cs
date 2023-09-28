// Copyright (c) Escendit Ltd. All Rights Reserved.
// Licensed under the MIT. See LICENSE.txt file in the solution root for full license information.

namespace Orleans.Hosting;

using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Clustering Silo Builder.
/// </summary>
internal class ClusteringSiloBuilder : IClusteringSiloBuilder
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ClusteringSiloBuilder"/> class.
    /// </summary>
    /// <param name="serviceCollection">The service collection.</param>
    public ClusteringSiloBuilder(IServiceCollection serviceCollection)
    {
        Services = serviceCollection;
    }

    /// <inheritdoc/>
    public IServiceCollection Services { get; }
}
