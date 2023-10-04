// Copyright (c) Escendit Ltd. All Rights Reserved.
// Licensed under the MIT. See LICENSE.txt file in the solution root for full license information.

namespace Escendit.Orleans.Clustering.Cassandra.Tests.Fixtures;

using Escendit.Extensions.Hosting.Cassandra;
using global::Orleans.Configuration;
using global::Orleans.Configuration.Overrides;
using global::Orleans.Runtime;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

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
            .AddLogging()
            .AddCassandraClientAsDefault(options =>
            {
                options
                    .Endpoints.Add("localhost");
                options.DefaultKeyspace = "test";
            })
            .AddSingleton<IMembershipTable>(sp =>
                new CassandraMembershipTable(
                    "Default",
                    sp.GetRequiredCassandraClient(),
                    sp.GetRequiredService<ILogger<CassandraMembershipTable>>(),
                    new CassandraClientOptions
                    {
                        Endpoints =
                        {
                            "localhost",
                        },
                        DefaultKeyspace = "test",
                    },
                    sp.GetProviderClusterOptions("Default").Value))
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
