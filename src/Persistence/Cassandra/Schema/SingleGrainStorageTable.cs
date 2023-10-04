// Copyright (c) Escendit Ltd. All Rights Reserved.
// Licensed under the MIT. See LICENSE.txt file in the solution root for full license information.

namespace Escendit.Orleans.Persistence.Cassandra.Schema;

/// <summary>
/// Single Grain Storage Table.
/// </summary>
public class SingleGrainStorageTable
{
    /// <summary>
    /// Gets or sets the type.
    /// </summary>
    /// <value>The type.</value>
    public byte[] Type { get; set; }

    /// <summary>
    /// Gets or sets the id.
    /// </summary>
    /// <value>The id.</value>
    public byte[] Id { get; set; }

    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    /// <value>The name.</value>
    public string Name { get; set; } = default!;

    /// <summary>
    /// Gets or sets the state.
    /// </summary>
    /// <value>The state.</value>
    public byte[] State { get; set; }

    /// <summary>
    /// Gets or sets the etag.
    /// </summary>
    /// <value>The etag.</value>
    public string Etag { get; set; } = default!;
}
