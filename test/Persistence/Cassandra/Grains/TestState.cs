// Copyright (c) Escendit Ltd. All Rights Reserved.
// Licensed under the MIT. See LICENSE.txt file in the solution root for full license information.

namespace Escendit.Orleans.Persistence.Cassandra.Tests.Grains;

/// <summary>
/// Test Stage.
/// </summary>
[GenerateSerializer]
public class TestState
{
    /// <summary>
    /// Gets or sets the state.
    /// </summary>
    /// <value>The state.</value>
    [Id(0)]
    public string? State { get; set; }
}
