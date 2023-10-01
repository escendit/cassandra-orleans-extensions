// Copyright (c) Escendit Ltd. All Rights Reserved.
// Licensed under the MIT. See LICENSE.txt file in the solution root for full license information.

namespace Escendit.Orleans.Persistence.Cassandra.Tests.Grains;

/// <summary>
/// Test Grain.
/// </summary>
public interface ITestGrain
{
    /// <summary>
    /// Read Simple State.
    /// </summary>
    /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
    Task<TestState> ReadState();

    /// <summary>
    /// Write Simple State.
    /// </summary>
    /// <param name="state">The state.</param>
    /// <returns>The task.</returns>
    Task WriteState(TestState state);

    /// <summary>
    /// Clear State.
    /// </summary>
    /// <returns>The task.</returns>
    Task ClearState();
}
