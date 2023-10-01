// Copyright (c) Escendit Ltd. All Rights Reserved.
// Licensed under the MIT. See LICENSE.txt file in the solution root for full license information.

namespace Escendit.Orleans.Persistence.Cassandra.Tests.Fixtures;

using Configuration;
using global::Orleans.TestingHost;

/// <summary>
/// Test Cluster Fixture.
/// </summary>
public sealed class TestClusterFixture : IDisposable
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TestClusterFixture"/> class.
    /// </summary>
    public TestClusterFixture()
    {
        var builder = new TestClusterBuilder
        {
            Options =
            {
                ClusterId = "testCluster",
                ServiceId = "testService",
            },
        };
        builder.AddSiloBuilderConfigurator<TestSiloConfigurator>();
        builder.AddClientBuilderConfigurator<TestClientBuilderConfigurator>();
        Cluster = builder.Build();
        Cluster.Deploy();
    }

    /// <summary>
    /// Finalizes an instance of the <see cref="TestClusterFixture"/> class.
    /// </summary>
    ~TestClusterFixture() => Dispose(false);

    /// <summary>
    /// Gets the cluster.
    /// </summary>
    /// <value>The cluster.</value>
    public TestCluster Cluster { get; }

    /// <inheritdoc/>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (disposing)
        {
            Cluster.StopAllSilos();
        }
    }
}
