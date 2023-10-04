// Copyright (c) Escendit Ltd. All Rights Reserved.
// Licensed under the MIT. See LICENSE.txt file in the solution root for full license information.

namespace Escendit.Orleans.Reminders.Cassandra.Mapping;

using global::Cassandra.Mapping;
using Schema;

/// <summary>
/// Reminder Mapping.
/// </summary>
public class ReminderMapping : Mappings
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ReminderMapping"/> class.
    /// </summary>
    public ReminderMapping()
    {
        For<Reminder>()
            .PartitionKey("type", "id", "name")
            .Column(p => p.Type, cm => cm.WithName("type"))
            .Column(p => p.Id, cm => cm.WithName("id"))
            .Column(p => p.Hash, cm => cm.WithName("hash").WithSecondaryIndex())
            .Column(p => p.Name, cm => cm.WithName("name"))
            .Column(p => p.StartOn, cm => cm.WithName("start_on"))
            .Column(p => p.Period, cm => cm.WithName("period"))
            .Column(p => p.Etag, cm => cm.WithName("etag"));
    }
}
