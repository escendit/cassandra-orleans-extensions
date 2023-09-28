// Copyright (c) Escendit Ltd. All Rights Reserved.
// Licensed under the MIT. See LICENSE.txt file in the solution root for full license information.

namespace Escendit.Orleans.Clustering.Cassandra;

using global::Orleans;
using global::Orleans.Messaging;
using global::Orleans.Runtime;
using Microsoft.Extensions.Options;
using Options;

/// <summary>
/// Cassandra Gateway List Provider.
/// </summary>
public class CassandraGatewayListProvider : IGatewayListProvider
{
    private readonly IMembershipTable _membershipTable;

    /// <summary>
    /// Initializes a new instance of the <see cref="CassandraGatewayListProvider"/> class.
    /// </summary>
    /// <param name="membershipTable">The membership table.</param>
    /// <param name="options">The options.</param>
    public CassandraGatewayListProvider(
        IMembershipTable membershipTable,
        IOptions<CassandraClusteringOptions> options)
    {
        ArgumentNullException.ThrowIfNull(membershipTable);
        ArgumentNullException.ThrowIfNull(options);
        _membershipTable = membershipTable;
        MaxStaleness = options.Value.MaxStaleness;
    }

    /// <inheritdoc/>
    public TimeSpan MaxStaleness { get; }

    /// <inheritdoc/>
    public bool IsUpdatable { get; } = true;

    /// <inheritdoc/>
    public Task InitializeGatewayListProvider()
    {
        return _membershipTable.InitializeMembershipTable(false);
    }

    /// <inheritdoc/>
    public async Task<IList<Uri>> GetGateways()
    {
        var membershipTableData = await _membershipTable.ReadAll();
        return membershipTableData
            .Members
            .Where(w => w.Item1.Status == SiloStatus.Active)
            .ToList()
            .ConvertAll(item => item
                .Item1
                .SiloAddress.ToGatewayUri());
    }
}
