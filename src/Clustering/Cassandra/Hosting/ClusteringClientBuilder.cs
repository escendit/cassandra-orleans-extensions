// Copyright (c) Escendit Ltd. All Rights Reserved.
// Licensed under the MIT. See LICENSE.txt file in the solution root for full license information.

namespace Orleans.Hosting;

using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Clustering Client Builder.
/// </summary>
internal class ClusteringClientBuilder : IClusteringClientBuilder
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ClusteringClientBuilder"/> class.
    /// </summary>
    /// <param name="serviceCollection">The service collection.</param>
    public ClusteringClientBuilder(IServiceCollection serviceCollection)
    {
        Services = serviceCollection;
    }

    /// <inheritdoc/>
    public IServiceCollection Services { get; }
}
