// Copyright (c) Escendit Ltd. All Rights Reserved.
// Licensed under the MIT. See LICENSE.txt file in the solution root for full license information.

namespace Escendit.Orleans.Clustering.Cassandra;

using global::Cassandra.Mapping;

/// <summary>
/// Membership Mapping.
/// </summary>
internal class MembershipMapping : Mappings
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MembershipMapping"/> class.
    /// </summary>
    public MembershipMapping()
    {
        For<Membership>()
            .TableName("membership")
            .ClusteringKey(p => p.Id)
            .ExplicitColumns()
            .Column(p => p.Id, cm => cm.WithName("id"))
            .Column(p => p.Etag, cm => cm.WithName("etag"))
            .Column(p => p.Silos, cm => cm.WithName("silos"))
            .Column(p => p.Version, cm => cm.WithName("version"));
    }
}
