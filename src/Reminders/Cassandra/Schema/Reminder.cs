// Copyright (c) Escendit Ltd. All Rights Reserved.
// Licensed under the MIT. See LICENSE.txt file in the solution root for full license information.

namespace Escendit.Orleans.Reminders.Cassandra.Schema;

/// <summary>
/// Reminder.
/// </summary>
public record Reminder
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Reminder"/> class.
    /// </summary>
    /// <param name="type">The grain id's type.</param>
    /// <param name="id">The grain id's id.</param>
    /// <param name="hash">The hash.</param>
    /// <param name="name">The name.</param>
    /// <param name="startOn">The start on.</param>
    /// <param name="period">The period.</param>
    /// <param name="etag">The etag.</param>
    public Reminder(byte[] type, byte[] id, long hash, string name, DateTimeOffset startOn, long period, string etag)
    {
        Type = type;
        Id = id;
        Hash = hash;
        Name = name;
        StartOn = startOn;
        Period = period;
        Etag = etag;
    }

    /// <summary>
    /// Gets or sets the type.
    /// </summary>
    /// <value>The type component.</value>
    public byte[] Type { get; set; }

    /// <summary>
    /// Gets or sets the id.
    /// </summary>
    /// <value>The id.</value>
    public byte[] Id { get; set; }

    /// <summary>
    /// Gets or sets the hash.
    /// </summary>
    /// <value>The hash.</value>
    public long Hash { get; set; }

    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    /// <value>The name.</value>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the start on.
    /// </summary>
    /// <value>The start on.</value>
    public DateTimeOffset StartOn { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// Gets or sets the period.
    /// </summary>
    /// <value>The period.</value>
    public long Period { get; set; }

    /// <summary>
    /// Gets or sets the etag.
    /// </summary>
    /// <value>The etag.</value>
    public string Etag { get; set; }
}
