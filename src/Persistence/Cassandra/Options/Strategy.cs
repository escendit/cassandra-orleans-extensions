// Copyright (c) Escendit Ltd. All Rights Reserved.
// Licensed under the MIT. See LICENSE.txt file in the solution root for full license information.

namespace Escendit.Orleans.Persistence.Cassandra.Options;

/// <summary>
/// Storage Strategy.
/// </summary>
public enum Strategy
{
    /// <summary>
    /// Store data into single table.
    /// </summary>
    SingleTable,

    /// <summary>
    /// Store data into multiple tables, each grain into its own table.
    /// </summary>
    TablePerGrain,
}
