// Copyright (c) Escendit Ltd. All Rights Reserved.
// Licensed under the MIT. See LICENSE.txt file in the solution root for full license information.

namespace Escendit.Orleans.Clustering.Cassandra.Tests;

using System.Net;
using Collections;
using Fixtures;
using global::Orleans;
using global::Orleans.Runtime;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Categories;

/// <summary>
/// Membership Table Tests.
/// </summary>
[Collection(SimulatorCollectionFixture.Name)]
public class MembershipTableTests
{
    private readonly SimulatorFixture _simulatorFixture;

    /// <summary>
    /// Initializes a new instance of the <see cref="MembershipTableTests"/> class.
    /// </summary>
    /// <param name="simulatorFixture">The simulator fixture.</param>
    public MembershipTableTests(SimulatorFixture simulatorFixture)
    {
        _simulatorFixture = simulatorFixture;
    }

    /// <summary>
    /// Test Connect.
    /// </summary>
    /// <returns>The task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task Test_ConnectAsync()
    {
        var membershipTable = _simulatorFixture.Services.GetRequiredService<IMembershipTable>();
        await membershipTable.InitializeMembershipTable(true);
        Assert.NotNull(membershipTable);
    }

    /// <summary>
    /// Test Inserting a row.
    /// </summary>
    /// <returns>The task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task Test_InsertRow_Simple()
    {
        var membershipTable = _simulatorFixture.Services.GetRequiredService<IMembershipTable>();
        await membershipTable.InitializeMembershipTable(false);
        Assert.NotNull(membershipTable);
    }

    /// <summary>
    /// Test updating a row.
    /// </summary>
    /// <param name="version">The version.</param>
    /// <returns>The task.</returns>
    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [IntegrationTest]
    public async Task Test_InsertRow_Sequence(int version)
    {
        var membershipTable = _simulatorFixture.Services.GetRequiredService<IMembershipTable>();
        await membershipTable.InitializeMembershipTable(false);
        await membershipTable.InsertRow(
            new MembershipEntry
            {
                Status = SiloStatus.Active,
                SiloAddress = SiloAddress.New(IPAddress.Loopback, 11111, 0),
                FaultZone = 0,
                HostName = "localhost",
                ProxyPort = 30000,
                RoleName = "Test",
                SiloName = "Test",
                StartTime = DateTime.UtcNow,
                UpdateZone = 0,
                IAmAliveTime = DateTime.UtcNow,
            },
            new TableVersion(version, Guid.NewGuid().ToString()));
        Assert.NotNull(membershipTable);
    }

    /// <summary>
    /// Test UpdateRow Simple.
    /// </summary>
    /// <returns>The task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task Test_UpdateRow_Simple()
    {
        var membershipTable = _simulatorFixture.Services.GetRequiredService<IMembershipTable>();
        await membershipTable.InitializeMembershipTable(false);
        await membershipTable.InsertRow(
            new MembershipEntry
            {
                Status = SiloStatus.Active,
                SiloAddress = SiloAddress.New(IPAddress.Loopback, 11111, 1),
                FaultZone = 0,
                HostName = "localhost",
                ProxyPort = 30000,
                RoleName = "Test",
                SiloName = "Test",
                StartTime = DateTime.UtcNow,
                UpdateZone = 0,
                IAmAliveTime = DateTime.UtcNow,
            },
            new TableVersion(3, Guid.NewGuid().ToString()));
        await membershipTable.UpdateRow(
            new MembershipEntry
            {
                Status = SiloStatus.Active,
                SiloAddress = SiloAddress.New(IPAddress.Loopback, 11111, 1),
                FaultZone = 0,
                HostName = "localhost",
                ProxyPort = 30000,
                RoleName = "Test",
                SiloName = "Test",
                StartTime = DateTime.UtcNow,
                UpdateZone = 0,
                IAmAliveTime = DateTime.UtcNow,
            },
            Guid.NewGuid().ToString(),
            new TableVersion(4, Guid.NewGuid().ToString()));
        Assert.NotNull(membershipTable);
    }
}
