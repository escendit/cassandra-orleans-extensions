// Copyright (c) Escendit Ltd. All Rights Reserved.
// Licensed under the MIT. See LICENSE.txt file in the solution root for full license information.

#pragma warning disable CA1812

namespace Escendit.Orleans.Persistence.Cassandra.Storage;

using Escendit.Extensions.Hosting.Cassandra;
using global::Cassandra;
using global::Cassandra.Data.Linq;
using global::Cassandra.Mapping;
using global::Orleans;
using global::Orleans.Runtime;
using Mapping;
using Microsoft.Extensions.Logging;
using Options;
using Schema;

/// <summary>
/// Single Table Grain Storage.
/// </summary>
internal class SingleTableGrainStorage : GrainStorageBase
{
    private readonly CassandraStorageOptions _storageOptions;
    private readonly MappingConfiguration _mappingConfiguration;
    private PreparedStatement? _readStatement;
    private PreparedStatement? _writeStatement;
    private PreparedStatement? _clearStatement;

    /// <summary>
    /// Initializes a new instance of the <see cref="SingleTableGrainStorage"/> class.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="logger">The logger.</param>
    /// <param name="serviceProvider">The service provider.</param>
    /// <param name="clientOptions">The client options.</param>
    /// <param name="storageOptions">The storage options.</param>
    public SingleTableGrainStorage(
        string name,
        ILogger<SingleTableGrainStorage> logger,
        IServiceProvider serviceProvider,
        CassandraClientOptions clientOptions,
        CassandraStorageOptions storageOptions)
        : base(name, logger, serviceProvider.GetRequiredCassandraClient(name), clientOptions, storageOptions)
    {
        _storageOptions = storageOptions;
        _mappingConfiguration = new MappingConfiguration()
            .Define<SingleGrainStorageMapping>();
    }

    /// <inheritdoc/>
    public override Task ReadStateAsync<T>(string stateName, GrainId grainId, IGrainState<T> grainState)
    {
        ArgumentNullException.ThrowIfNull(stateName);
        ArgumentNullException.ThrowIfNull(grainId);
        ArgumentNullException.ThrowIfNull(grainState);
        ArgumentNullException.ThrowIfNull(Session);
        return ReadStateInternalAsync(stateName, grainId, grainState);
    }

    /// <inheritdoc/>
    public override Task WriteStateAsync<T>(string stateName, GrainId grainId, IGrainState<T> grainState)
    {
        ArgumentNullException.ThrowIfNull(stateName);
        ArgumentNullException.ThrowIfNull(grainId);
        ArgumentNullException.ThrowIfNull(grainState);
        ArgumentNullException.ThrowIfNull(Session);
        return WriteStateInternalAsync(stateName, grainId, grainState);
    }

    /// <inheritdoc/>
    public override Task ClearStateAsync<T>(string stateName, GrainId grainId, IGrainState<T> grainState)
    {
        ArgumentNullException.ThrowIfNull(stateName);
        ArgumentNullException.ThrowIfNull(grainId);
        ArgumentNullException.ThrowIfNull(grainState);
        ArgumentNullException.ThrowIfNull(Session);
        return ClearStateInternalAsync(stateName, grainId, grainState);
    }

    /// <inheritdoc/>
    protected override async Task Initialize(CancellationToken cancellationToken)
    {
        await base.Initialize(cancellationToken);

        var storage =
            new Table<SingleGrainStorageTable>(Session, _mappingConfiguration, _storageOptions.TableNameOrPrefix);
        await Execute(() => storage.CreateIfNotExistsAsync());

        _readStatement =
            await Execute(
                () => Session!.PrepareAsync(
                    $"""
                     SELECT name, type, id, state, etag
                     FROM "{_storageOptions.TableNameOrPrefix}"
                     WHERE name = ? AND type = ? AND id = ?
                     """));
        _writeStatement =
            await Execute(
                () => Session!.PrepareAsync(
                    $"""
                     INSERT INTO "{_storageOptions.TableNameOrPrefix}" (name, type, id, state, etag)
                     VALUES (?, ?, ?, ?, ?)
                     """));
        _clearStatement =
            await Execute(
                () => Session!.PrepareAsync(
                    $"""
                     DELETE FROM "{_storageOptions.TableNameOrPrefix}"
                     WHERE name = ? AND type = ? AND id = ?
                     """));
    }

    private async Task ReadStateInternalAsync<T>(string stateName, GrainId grainId, IGrainState<T> grainState)
    {
        var name = GenerateStateName<T>(stateName, grainId);
        var type = GenerateTypeName<T>(stateName, grainId);
        var id = GenerateId<T>(stateName, grainId);
        var results = await Execute(() =>
            Session!.ExecuteAsync(_readStatement!.Bind(name, type, id)));

        if (results.GetAvailableWithoutFetching() == 0)
        {
            grainState.State = default!;
            grainState.ETag = default;
            grainState.RecordExists = default;
            return;
        }

        var result = results.First();
        var state = result.GetValue<byte[]>("state");
        grainState.State = _storageOptions.GrainStorageSerializer!.Deserialize<T>(new BinaryData(state));
        grainState.ETag = result.GetValue<string>("etag");
        grainState.RecordExists = true;
    }

    private async Task WriteStateInternalAsync<T>(string stateName, GrainId grainId, IGrainState<T> grainState)
    {
        var name = GenerateStateName<T>(stateName, grainId);
        var type = GenerateTypeName<T>(stateName, grainId);
        var id = GenerateId<T>(stateName, grainId);
        var state = _storageOptions.GrainStorageSerializer!.Serialize(grainState.State).ToArray();
        var etag = grainState.ETag;
        await Execute(() =>
            Session!.ExecuteAsync(_writeStatement!.Bind(name, type, id, state, etag)));
    }

    private async Task ClearStateInternalAsync<T>(string stateName, GrainId grainId, IGrainState<T> grainState)
    {
        if (!_storageOptions.DeleteStateOnClear)
        {
            return;
        }

        var name = GenerateStateName<T>(stateName, grainId);
        var type = GenerateTypeName<T>(stateName, grainId);
        var id = GenerateId<T>(stateName, grainId);
        var results = await Execute(() =>
            Session!.ExecuteAsync(_clearStatement!.Bind(name, type, id)));

        if (results.GetAvailableWithoutFetching() != 0)
        {
            grainState.State = default!;
            grainState.ETag = default;
            grainState.RecordExists = default;
        }
    }
}
