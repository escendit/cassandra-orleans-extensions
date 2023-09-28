// Copyright (c) Escendit Ltd. All Rights Reserved.
// Licensed under the MIT. See LICENSE.txt file in the solution root for full license information.

namespace Escendit.Orleans.Persistence.Cassandra.Tests.Configuration;

using global::Orleans.TestingHost;
using Microsoft.Extensions.Logging;

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
            .UseCassandraClustering()
            .WithClientAsDefault(options =>
            {
                options.Endpoints.Add("localhost");
                options.DefaultKeyspace = "test";
            })
            .WithOptions(options => options.MaxStaleness = TimeSpan.FromDays(1));
    }
}
