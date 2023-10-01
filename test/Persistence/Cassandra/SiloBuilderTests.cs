// Copyright (c) Escendit Ltd. All Rights Reserved.
// Licensed under the MIT. See LICENSE.txt file in the solution root for full license information.

namespace Escendit.Orleans.Persistence.Cassandra.Tests;

using Collections;
using Fixtures;
using global::Orleans.TestingHost;
using Grains;
using Xunit;
using Xunit.Categories;

/// <summary>
/// Silo Builder Tests.
/// </summary>
[Collection(SiloCollectionFixture.Name)]
public class SiloBuilderTests
{
    private readonly TestCluster _cluster;

    /// <summary>
    /// Initializes a new instance of the <see cref="SiloBuilderTests"/> class.
    /// </summary>
    /// <param name="fixture">The fixture.</param>
    /// <param name="testClusterFixture">the test cluster.</param>
    public SiloBuilderTests(SiloBuilderFixture fixture, TestClusterFixture testClusterFixture)
    {
        ArgumentNullException.ThrowIfNull(fixture);
        ArgumentNullException.ThrowIfNull(testClusterFixture);
        _cluster = testClusterFixture.Cluster;
    }

    /// <summary>
    /// Acquire Simple Guid Grain.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    [IntegrationTest]
    public async Task Test_AcquireSimpleGuidGrain()
    {
        var grain = _cluster.GrainFactory.GetGrain<ISimpleGuidGrain>(Guid.Empty);
        await grain.ReadState();
        await grain.WriteState(new TestState
        {
            State = "Hello World!",
        });
        var state = await grain.ReadState();
        Assert.NotNull(grain);
        Assert.Equal("Hello World!", state.State);
        await grain.ClearState();
    }

    /// <summary>
    /// Acquire Simple Integer Grain.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    [IntegrationTest]
    public async Task Test_AcquireSimpleIntegerGrain()
    {
        var grain = _cluster.GrainFactory.GetGrain<ISimpleIntegerGrain>(0);
        await grain.ReadState();
        await grain.WriteState(new TestState
        {
            State = "Hello World!",
        });
        var state = await grain.ReadState();
        Assert.NotNull(grain);
        Assert.Equal("Hello World!", state.State);
        await grain.ClearState();
    }

    /// <summary>
    /// Acquire Simple Integer Grain.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    [IntegrationTest]
    public async Task Test_AcquireSimpleStringGrain()
    {
        var grain = _cluster.GrainFactory.GetGrain<ISimpleStringGrain>("random_string_here");
        await grain.ReadState();
        await grain.WriteState(new TestState
        {
            State = "Hello World!",
        });
        var state = await grain.ReadState();
        Assert.NotNull(grain);
        Assert.Equal("Hello World!", state.State);
        /* await grain.ClearState(); */
    }
}
