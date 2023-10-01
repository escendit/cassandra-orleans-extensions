// Copyright (c) Escendit Ltd. All Rights Reserved.
// Licensed under the MIT. See LICENSE.txt file in the solution root for full license information.

namespace Escendit.Orleans.Reminders.Cassandra.Tests.Grains;

using global::Orleans.Runtime;
using Microsoft.Extensions.Logging;

/// <summary>
/// Test Grain.
/// </summary>
public class TestGrain : IGrainBase, ITestGrain, IRemindable
{
    private readonly ILogger _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="TestGrain"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="grainContext">The grain context.</param>
    public TestGrain(ILogger<TestGrain> logger, IGrainContext grainContext)
    {
        _logger = logger;
        GrainContext = grainContext;
    }

    /// <inheritdoc/>
    public IGrainContext GrainContext { get; }

    /// <inheritdoc/>
    public Task RemindMe(string message)
    {
        ArgumentNullException.ThrowIfNull(message);
        return this.RegisterOrUpdateReminder("rememberMe", TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(1));
    }

    /// <inheritdoc/>
    public Task ReceiveReminder(string reminderName, TickStatus status)
    {
        _logger.LogInformation("Reminder triggered for {Name}: {Tick1} {Tick2}", reminderName, status.FirstTickTime, status.CurrentTickTime);
        return Task.CompletedTask;
    }
}
