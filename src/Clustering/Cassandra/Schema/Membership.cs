// Copyright (c) Escendit Ltd. All Rights Reserved.
// Licensed under the MIT. See LICENSE.txt file in the solution root for full license information.

namespace Escendit.Orleans.Clustering.Cassandra;

/// <summary>
/// Membership.
/// </summary>
public class Membership
{
    /// <summary>
    /// Gets or sets the id.
    /// </summary>
    /// <value>The id.</value>
    public string Id { get; set; } = default!;

    /// <summary>
    /// Gets the silos.
    /// </summary>
    /// <value>The silos.</value>
    public IDictionary<string, Silo> Silos { get; init; } = new Dictionary<string, Silo>();

    /// <summary>
    /// Gets or sets the version.
    /// </summary>
    /// <value>The version.</value>
    public int Version { get; set; }

    /// <summary>
    /// Gets or sets the etag.
    /// </summary>
    /// <value>The etag.</value>
    public string Etag { get; set; } = Guid.Empty.ToString();
}
