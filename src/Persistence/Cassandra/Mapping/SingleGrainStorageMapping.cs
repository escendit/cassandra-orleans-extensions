// Copyright (c) Escendit Ltd. All Rights Reserved.
// Licensed under the MIT. See LICENSE.txt file in the solution root for full license information.

namespace Escendit.Orleans.Persistence.Cassandra.Mapping;

using global::Cassandra.Mapping;
using Schema;

/// <summary>
/// Single Grain Storage Mapping.
/// </summary>
internal class SingleGrainStorageMapping : Mappings
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SingleGrainStorageMapping"/> class.
    /// </summary>
    public SingleGrainStorageMapping()
    {
        For<SingleGrainStorageTable>()
            .PartitionKey("type", "id", "name")
            .Column(p => p.Type, cm => cm.WithName("type"))
            .Column(p => p.Id, cm => cm.WithName("id"))
            .Column(p => p.Name, cm => cm.WithName("name"))
            .Column(p => p.State, cm => cm.WithName("state"))
            .Column(p => p.Etag, cm => cm.WithName("etag"));
    }
}
