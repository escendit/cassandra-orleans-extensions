// Copyright (c) Escendit Ltd. All Rights Reserved.
// Licensed under the MIT. See LICENSE.txt file in the solution root for full license information.

namespace Escendit.Orleans.Clustering.Cassandra.Tests.Fixtures;

using global::Orleans.Configuration;
using global::Orleans.Runtime;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Options;

/// <summary>
/// Simulator Fixture.
/// </summary>
public sealed class SimulatorFixture
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SimulatorFixture"/> class.
    /// </summary>
    public SimulatorFixture()
    {
        var services = new ServiceCollection()
            .AddCassandraClientAsDefault(options =>
            {
                options
                    .Endpoints.Add("localhost");
                options.DefaultKeyspace = "test";
            })
            .Configure<ClusterOptions>(options => options.ClusterId = "default");

        services
            .TryAddSingleton(typeof(IKeyedServiceCollection<,>), typeof(KeyedServiceCollection<,>));
        Services = services.BuildServiceProvider();
    }

    /// <summary>
    /// Gets the services.
    /// </summary>
    /// <value>The services.</value>
    public IServiceProvider Services { get; }
}
