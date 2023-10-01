// Copyright (c) Escendit Ltd. All Rights Reserved.
// Licensed under the MIT. See LICENSE.txt file in the solution root for full license information.

namespace Escendit.Orleans.Persistence.Cassandra.Tests.Configuration;

using global::Orleans.TestingHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Options;

/// <summary>
/// Test Silo Configurator.
/// </summary>
public class TestSiloConfigurator : ISiloConfigurator
{
    /// <inheritdoc/>
    public void Configure(ISiloBuilder siloBuilder)
    {
        siloBuilder
            .ConfigureLogging(options =>
            {
                options.AddConsole();
                options.SetMinimumLevel(LogLevel.Debug);
            })
            .ConfigureServices(services => services
                .AddCassandraClientAsDefault(options =>
                {
                    options.Endpoints.Add("localhost");
                    options.DefaultKeyspace = "test";
                }))
            .AddCassandraGrainStorageAsDefault(builder =>
            {
                builder.Strategy = Strategy.SingleTable;
                builder.TableNameOrPrefix = "Default";
            });
    }
}
