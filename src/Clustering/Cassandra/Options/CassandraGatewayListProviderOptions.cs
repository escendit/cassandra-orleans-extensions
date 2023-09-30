// Copyright (c) Escendit Ltd. All Rights Reserved.
// Licensed under the MIT. See LICENSE.txt file in the solution root for full license information.

namespace Escendit.Orleans.Clustering.Cassandra.Options;

using global::Orleans.Messaging;

/// <summary>
/// Clustering Gateway List Provider Options.
/// </summary>
public record CassandraGatewayListProviderOptions
{
    /// <summary>
    /// Gets or sets the max staleness.
    /// </summary>
    /// <value>The maximum staleness period.</value>
    /// <seealso cref="IGatewayListProvider.MaxStaleness"/>
    public TimeSpan MaxStaleness { get; set; }
}
