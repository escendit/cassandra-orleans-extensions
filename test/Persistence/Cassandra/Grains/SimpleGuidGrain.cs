// Copyright (c) Escendit Ltd. All Rights Reserved.
// Licensed under the MIT. See LICENSE.txt file in the solution root for full license information.

namespace Escendit.Orleans.Persistence.Cassandra.Tests.Grains;

using global::Orleans.Runtime;

/// <summary>
/// Simple Guid Grain.
/// </summary>
public class SimpleGuidGrain : Grain, ISimpleGuidGrain
{
    private readonly IPersistentState<TestState> _persistentState;

    /// <summary>
    /// Initializes a new instance of the <see cref="SimpleGuidGrain"/> class.
    /// </summary>
    /// <param name="persistentState">The persistent state.</param>
    public SimpleGuidGrain([PersistentState("SimpleGuidGrain")] IPersistentState<TestState> persistentState)
    {
        _persistentState = persistentState;
    }

    /// <inheritdoc/>
    public async Task<TestState> ReadState()
    {
        await _persistentState.ReadStateAsync();
        return _persistentState.State;
    }

    /// <inheritdoc/>
    public async Task WriteState(TestState state)
    {
        _persistentState.State = state;
        await _persistentState.WriteStateAsync();
    }

    /// <inheritdoc/>
    public Task ClearState()
    {
        return _persistentState.ClearStateAsync();
    }
}
