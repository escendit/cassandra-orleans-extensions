// Copyright (c) Escendit Ltd. All Rights Reserved.
// Licensed under the MIT. See LICENSE.txt file in the solution root for full license information.

namespace Escendit.Orleans.Reminders.Cassandra.Tests.Grains;

/// <summary>
/// Test Grain.
/// </summary>
public interface ITestGrain : IGrainWithIntegerKey
{
    /// <summary>
    /// Remind me with message.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
    public Task RemindMe(string message);

    /// <summary>
    /// Forget Me.
    /// </summary>
    /// <returns>The task.</returns>
    public Task ForgetMe();
}
