// Copyright (c) Escendit Ltd. All Rights Reserved.
// Licensed under the MIT. See LICENSE.txt file in the solution root for full license information.

namespace Escendit.Orleans.Persistence.Cassandra.Options;

using global::Orleans;

/// <summary>
/// Cassandra Storage Options Validator.
/// </summary>
public class CassandraStorageOptionsValidator : IConfigurationValidator
{
    private readonly CassandraStorageOptions _options;
    private readonly string _name;

    /// <summary>
    /// Initializes a new instance of the <see cref="CassandraStorageOptionsValidator"/> class.
    /// </summary>
    /// <param name="options">The options.</param>
    /// <param name="name">The name.</param>
    public CassandraStorageOptionsValidator(CassandraStorageOptions options, string name)
    {
        _options = options;
        _name = name;
    }

    /// <inheritdoc/>
    public void ValidateConfiguration()
    {
    }
}
