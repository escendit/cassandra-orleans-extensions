// Copyright (c) Escendit Ltd. All Rights Reserved.
// Licensed under the MIT. See LICENSE.txt file in the solution root for full license information.

namespace Escendit.Orleans.Clustering.Cassandra;

using global::Orleans.Messaging;

/// <summary>
/// Cassandra Gateway List Provider.
/// </summary>
public class CassandraGatewayListProvider : IGatewayListProvider
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CassandraGatewayListProvider"/> class.
    /// </summary>
    public CassandraGatewayListProvider()
    {
    }

    /// <inheritdoc/>
    public TimeSpan MaxStaleness { get; } = TimeSpan.Zero;

    /// <inheritdoc/>
    public bool IsUpdatable { get; } = true;

    /// <inheritdoc/>
    public Task InitializeGatewayListProvider()
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public Task<IList<Uri>> GetGateways()
    {
        throw new NotImplementedException();
    }
}
