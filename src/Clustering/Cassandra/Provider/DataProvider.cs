// Copyright (c) Escendit Ltd. All Rights Reserved.
// Licensed under the MIT. See LICENSE.txt file in the solution root for full license information.

namespace Escendit.Orleans.Clustering.Cassandra;

using Microsoft.Extensions.Logging;

/// <summary>
/// Data Provider.
/// </summary>
public partial class DataProvider
{
    private readonly ILogger _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="DataProvider"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    public DataProvider(
        ILogger<DataProvider> logger)
    {
        _logger = logger;
    }

    [LoggerMessage(
        EventId = 100,
        EventName = "",
        Level = LogLevel.Information,
        Message = "Fine")]
    private partial void LogSomething();
}
