// Copyright (c) Escendit Ltd. All Rights Reserved.
// Licensed under the MIT. See LICENSE.txt file in the solution root for full license information.

namespace Escendit.Orleans.Persistence.Cassandra.Tests.Configuration;

using global::Orleans.TestingHost;
using Microsoft.Extensions.Configuration;

/// <summary>
/// Test Client Builder Configurator.
/// </summary>
public class TestClientBuilderConfigurator : IClientBuilderConfigurator
{
    /// <inheritdoc/>
    public void Configure(IConfiguration configuration, IClientBuilder clientBuilder)
    {
        clientBuilder
            .UseCassandraClustering()
            .WithClientAsDefault(options =>
            {
                options.Endpoints.Add("localhost");
                options.DefaultKeyspace = "test";
            })
            .Build();
    }
}
