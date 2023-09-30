// Copyright (c) Escendit Ltd. All Rights Reserved.
// Licensed under the MIT. See LICENSE.txt file in the solution root for full license information.

namespace Escendit.Orleans.Clustering.Cassandra.Tests;

using Collections;
using Escendit.Orleans.Persistence.Cassandra.Tests.Fixtures;
using Xunit;
using Xunit.Categories;

/// <summary>
/// Test Cluster Tests.
/// </summary>
[Collection(ClusterCollectionFixture.Name)]
public class TestClusterTests
{
    private readonly TestClusterFixture _testClusterFixture;

    /// <summary>
    /// Initializes a new instance of the <see cref="TestClusterTests"/> class.
    /// </summary>
    /// <param name="testClusterFixture">The cluster collection fixture.</param>
    public TestClusterTests(TestClusterFixture testClusterFixture)
    {
        _testClusterFixture = testClusterFixture;
    }

    /// <summary>
    /// Test Cluster.
    /// </summary>
    /// <returns>The task.</returns>
    /*[Fact]
    [IntegrationTest]
    public Task Test_Cluster()
    {
        var cluster = _testClusterFixture.Cluster;
        Assert.NotNull(cluster);
        return Task.CompletedTask;
    }*/

    /// <summary>
    /// Test Client.
    /// </summary>
    /// <returns>The task.</returns>
    [Fact]
    [IntegrationTest]
    public Task Test_Client()
    {
        var client = _testClusterFixture.Cluster.Client;
        Assert.NotNull(client);
        return Task.CompletedTask;
    }
}
