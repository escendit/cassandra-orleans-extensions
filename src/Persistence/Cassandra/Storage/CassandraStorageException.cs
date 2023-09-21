// Copyright (c) Escendit Ltd. All Rights Reserved.
// Licensed under the MIT. See LICENSE.txt file in the solution root for full license information.

namespace Escendit.Orleans.Persistence.Cassandra.Storage;

using System.Runtime.Serialization;
using global::Orleans;

/// <summary>
/// Cassandra Storage Exception.
/// </summary>
[GenerateSerializer]
public class CassandraStorageException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CassandraStorageException"/> class.
    /// </summary>
    public CassandraStorageException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CassandraStorageException"/> class.
    /// </summary>
    /// <param name="message">The message.</param>
    public CassandraStorageException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CassandraStorageException"/> class.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="innerException">The inner exception.</param>
    public CassandraStorageException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CassandraStorageException"/> class.
    /// </summary>
    /// <param name="serializationInfo">The serialization info.</param>
    /// <param name="streamingContext">The streaming context.</param>
    protected CassandraStorageException(SerializationInfo serializationInfo, StreamingContext streamingContext)
        : base(serializationInfo, streamingContext)
    {
    }
}
