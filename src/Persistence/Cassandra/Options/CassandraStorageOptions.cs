// Copyright (c) Escendit Ltd. All Rights Reserved.
// Licensed under the MIT. See LICENSE.txt file in the solution root for full license information.

namespace Escendit.Orleans.Persistence.Cassandra.Options;

using global::Orleans;
using global::Orleans.Storage;

/// <summary>
/// Cassandra Storage Options.
/// </summary>
public class CassandraStorageOptions : IStorageProviderSerializerOptions
{
    /// <summary>
    /// Gets or sets a value indicating whether delete state on clear.
    /// </summary>
    /// <value>The flag to delete state on clear.</value>
    public bool DeleteStateOnClear { get; set; }

    /// <summary>
    /// Gets or sets the initial stage.
    /// </summary>
    /// <value>The initial stage.</value>
    public int InitialStage { get; set; } = ServiceLifecycleStage.ApplicationServices;

    /// <inheritdoc/>
    public IGrainStorageSerializer? GrainStorageSerializer { get; set; }

    /// <summary>
    /// Gets or sets the strategy.
    /// </summary>
    /// <value>The strategy.</value>
    public Strategy? Strategy { get; set; } = Options.Strategy.SingleTable;

    /// <summary>
    /// Gets or sets the table name or prefix.
    /// </summary>
    /// <value>The table name or prefix.</value>
    public string? TableNameOrPrefix { get; set; }
}
