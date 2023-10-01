// Copyright (c) Escendit Ltd. All Rights Reserved.
// Licensed under the MIT. See LICENSE.txt file in the solution root for full license information.

namespace Escendit.Orleans.Persistence.Cassandra.Hosting;

using global::Orleans.Storage;
using Options;

/// <summary>
/// Default Cassandra Post Configure Options.
/// </summary>
public class DefaultCassandraPostConfigureOptions : DefaultStorageProviderSerializerOptionsConfigurator<CassandraStorageOptions>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultCassandraPostConfigureOptions"/> class.
    /// </summary>
    /// <param name="serviceProvider">The service provider.</param>
    public DefaultCassandraPostConfigureOptions(IServiceProvider serviceProvider)
        : base(serviceProvider)
    {
    }
}
