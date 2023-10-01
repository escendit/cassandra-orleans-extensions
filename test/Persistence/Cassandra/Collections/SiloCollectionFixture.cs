// Copyright (c) Escendit Ltd. All Rights Reserved.
// Licensed under the MIT. See LICENSE.txt file in the solution root for full license information.

namespace Escendit.Orleans.Persistence.Cassandra.Tests.Collections;

using Fixtures;
using Xunit;

/// <summary>
/// Silo Collection Fixture.
/// </summary>
[CollectionDefinition(Name)]
public class SiloCollectionFixture : ICollectionFixture<SiloBuilderFixture>, ICollectionFixture<TestClusterFixture>
{
    /// <summary>
    /// Silo Connection Name.
    /// </summary>
    public const string Name = "SiloCollection";
}
