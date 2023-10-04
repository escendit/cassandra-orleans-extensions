// Copyright (c) Escendit Ltd. All Rights Reserved.
// Licensed under the MIT. See LICENSE.txt file in the solution root for full license information.

namespace Escendit.Orleans.Clustering.Cassandra;

/// <summary>
/// Silo.
/// </summary>
internal class Silo
{
    /// <summary>
    /// Gets or sets the address.
    /// </summary>
    /// <value>The address.</value>
    public string Address { get; set; } = default!;

    /// <summary>
    /// Gets or sets the alive on.
    /// </summary>
    /// <value>The alive on.</value>
    public DateTimeOffset AliveOn { get; set; }

    /// <summary>
    /// Gets or sets the etag.
    /// </summary>
    /// <value>The etag.</value>
    public string Etag { get; set; } = default!;

    /// <summary>
    /// Gets or sets the fault zone.
    /// </summary>
    /// <value>The fault zone.</value>
    public int FaultZone { get; set; }

    /// <summary>
    /// Gets or sets the host name.
    /// </summary>
    /// <value>The host name.</value>
    public string HostName { get; set; } = default!;

    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    /// <value>The name.</value>
    public string Name { get; set; } = default!;

    /// <summary>
    /// Gets or sets the proxy port.
    /// </summary>
    /// <value>The proxy port.</value>
    public int ProxyPort { get; set; } = 30000;

    /// <summary>
    /// Gets or sets the started on.
    /// </summary>
    /// <value>The started on.</value>
    public DateTimeOffset StartedOn { get; set; }

    /// <summary>
    /// Gets or sets the role.
    /// </summary>
    /// <value>The role.</value>
    public string? Role { get; set; }

    /// <summary>
    /// Gets or sets the status.
    /// </summary>
    /// <value>The status.</value>
    public int Status { get; set; }

    /// <summary>
    /// Gets the suspect times.
    /// </summary>
    /// <value>The suspect times.</value>
    public IList<SuspectTime> SuspectTimes { get; init; } = new List<SuspectTime>();

    /// <summary>
    /// Gets or sets the update zone.
    /// </summary>
    /// <value>The update zone.</value>
    public int UpdateZone { get; set; }

    /// <summary>
    /// Gets or sets the timestamp.
    /// </summary>
    /// <value>The timestamp.</value>
    public DateTimeOffset Timestamp { get; set; }
}
