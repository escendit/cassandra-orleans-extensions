// Copyright (c) Escendit Ltd. All Rights Reserved.
// Licensed under the MIT. See LICENSE.txt file in the solution root for full license information.

namespace Escendit.Orleans.Reminders.Cassandra.Tests;

using Collections;
using Fixtures;
using Grains;

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
    /// <param name="testClusterFixture">The test cluster fixture.</param>
    public TestClusterTests(TestClusterFixture testClusterFixture)
    {
        _testClusterFixture = testClusterFixture;
    }

    /// <summary>
    /// Test spin up cluster.
    /// </summary>
    [Fact]
    public void Test_Load()
    {
        Assert.NotNull(_testClusterFixture.Cluster);
    }

    /// <summary>
    /// Test Reminder.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
    [Fact]
    public async Task Test_Reminder()
    {
        var cluster = _testClusterFixture.Cluster;
        var testGrain = cluster.GrainFactory.GetGrain<ITestGrain>(0);
        await testGrain.RemindMe("test");
        Assert.NotNull(testGrain);
        await Task.Delay(TimeSpan.FromSeconds(70));
        Assert.NotNull(testGrain);
    }
}
