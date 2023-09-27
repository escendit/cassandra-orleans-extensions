// Copyright (c) Escendit Ltd. All Rights Reserved.
// Licensed under the MIT. See LICENSE.txt file in the solution root for full license information.

namespace Escendit.Orleans.Clustering.Cassandra.Tests.Collections;

using Fixtures;
using Xunit;

/// <summary>
/// Simulator Collection Fixture.
/// </summary>
[CollectionDefinition(Name)]
public class SimulatorCollectionFixture : ICollectionFixture<SimulatorFixture>
{
    /// <summary>
    /// Name.
    /// </summary>
    public const string Name = "SimulatorCollection";
}
