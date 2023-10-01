// Copyright (c) Escendit Ltd. All Rights Reserved.
// Licensed under the MIT. See LICENSE.txt file in the solution root for full license information.

#pragma warning disable CA1812

namespace Escendit.Orleans.Persistence.Cassandra.Options;

using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Silo Persistence Builder.
/// </summary>
internal class SiloPersistenceBuilder : ISiloPersistenceBuilder
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SiloPersistenceBuilder"/> class.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="name">The name.</param>
    public SiloPersistenceBuilder(IServiceCollection services, string name)
    {
        Services = services;
        Name = name;
    }

    /// <summary>
    /// Gets the services.
    /// </summary>
    /// <value>The services.</value>
    public IServiceCollection Services { get; }

    /// <summary>
    /// Gets the name.
    /// </summary>
    /// <value>The name.</value>
    public string Name { get; }
}
