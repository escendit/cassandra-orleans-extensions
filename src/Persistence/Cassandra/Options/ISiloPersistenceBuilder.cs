// Copyright (c) Escendit Ltd. All Rights Reserved.
// Licensed under the MIT. See LICENSE.txt file in the solution root for full license information.

namespace Escendit.Orleans.Persistence.Cassandra;

using global::Orleans.Hosting;

/// <summary>
/// Silo Persistence Builder.
/// </summary>
public interface ISiloPersistenceBuilder : ISiloBuilder
{
    /// <summary>
    /// Gets the client name.
    /// </summary>
    /// <value>The client name.</value>
    public string Name { get; }
}
