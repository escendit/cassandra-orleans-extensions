// Copyright (c) Escendit Ltd. All Rights Reserved.
// Licensed under the MIT. See LICENSE.txt file in the solution root for full license information.

namespace Escendit.Orleans.Clustering.Cassandra.Options;

/// <summary>
/// Cassandra Clustering Options.
/// </summary>
public record CassandraClusteringOptions
{
    /// <summary>
    /// Gets or sets the client name.
    /// </summary>
    /// <value>The client name.</value>
    public string? ClientName { get; set; }
}
