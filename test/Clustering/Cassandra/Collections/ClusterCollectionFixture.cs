// Copyright (c) Escendit Ltd. All Rights Reserved.
// Licensed under the MIT. See LICENSE.txt file in the solution root for full license information.

namespace Escendit.Orleans.Clustering.Cassandra.Tests.Collections;

using Escendit.Orleans.Persistence.Cassandra.Tests.Fixtures;
using Xunit;

/// <summary>
/// Cluster Collection Fixture.
/// </summary>
[CollectionDefinition(Name)]
public class ClusterCollectionFixture : ICollectionFixture<TestClusterFixture>
{
    /// <summary>
    /// Name.
    /// </summary>
    public const string Name = "ClusterCollection";
}
