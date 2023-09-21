// Copyright (c) Escendit Ltd. All Rights Reserved.
// Licensed under the MIT. See LICENSE.txt file in the solution root for full license information.

namespace Escendit.Orleans.Clustering.Cassandra;

using global::Orleans;
using global::Orleans.Runtime;

/// <summary>
/// Cassandra Membership Table.
/// </summary>
public class CassandraMembershipTable : IMembershipTable
{
    /// <inheritdoc/>
    public Task InitializeMembershipTable(bool tryInitTableVersion)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public Task DeleteMembershipTableEntries(string clusterId)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public Task CleanupDefunctSiloEntries(DateTimeOffset beforeDate)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public Task<MembershipTableData> ReadRow(SiloAddress key)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public Task<MembershipTableData> ReadAll()
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public Task<bool> InsertRow(MembershipEntry entry, TableVersion tableVersion)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public Task<bool> UpdateRow(MembershipEntry entry, string etag, TableVersion tableVersion)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public Task UpdateIAmAlive(MembershipEntry entry)
    {
        throw new NotImplementedException();
    }
}
