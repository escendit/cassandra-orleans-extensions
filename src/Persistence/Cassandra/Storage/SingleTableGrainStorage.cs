﻿// Copyright (c) Escendit Ltd. All Rights Reserved.
// Licensed under the MIT. See LICENSE.txt file in the solution root for full license information.

#pragma warning disable CA1812

namespace Escendit.Orleans.Persistence.Cassandra.Storage;

using Escendit.Extensions.Hosting.Cassandra;
using global::Cassandra;
using global::Orleans;
using global::Orleans.Runtime;
using Microsoft.Extensions.Logging;
using Options;

/// <summary>
/// Single Table Grain Storage.
/// </summary>
internal partial class SingleTableGrainStorage : GrainStorageBase
{
    private readonly ILogger _logger;
    private readonly CassandraClientOptions _clientOptions;
    private readonly CassandraStorageOptions _storageOptions;
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
        : base(name, logger, serviceProvider.GetRequiredCassandraClient(name), clientOptions)
    {
        _logger = logger;
        _clientOptions = clientOptions;
        _storageOptions = storageOptions;
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

        var results = await Execute(
            () => Session!
                .ExecuteAsync(
                    new SimpleStatement(
                        "SELECT table_name FROM system_schema.tables WHERE keyspace_name = ? AND table_name = ? ALLOW FILTERING",
                        _clientOptions.DefaultKeyspace!,
                        _storageOptions.TableNameOrPrefix)));

        if (results.GetAvailableWithoutFetching() == 0)
        {
            var result = await Execute(
                () => Session!.ExecuteAsync(
                    new SimpleStatement(
                        $"""
                         create table "{_storageOptions.TableNameOrPrefix}" (
                             name   varchar,
                             type   blob,
                             id     blob,
                             state  blob,
                             etag   varchar
                             primary key ((name, type, id))
                         )
                         """)));

            if (result.GetAvailableWithoutFetching() != 0)
            {
                LogTableCreated(_storageOptions.TableNameOrPrefix!);
            }
        }

        _readStatement =
            await Execute(
                () => Session!.PrepareAsync(
                    $"select name, type, id, state, etag, exists from \"{_storageOptions.TableNameOrPrefix}\" where name = :name and type = :type and id = :id ALLOW FILTERING"));
        _writeStatement =
            await Execute(
                () => Session!.PrepareAsync(
                    $"insert into \"{_storageOptions.TableNameOrPrefix}\" (name, type, id, state, etag) VALUES (:name, :type, :id, :state, :etag)"));
        _clearStatement =
            await Execute(
                () => Session!.PrepareAsync(
                    $"delete from \"{_storageOptions.TableNameOrPrefix}\" where name = :name and type = :type and id = :id"));
    }

    private async Task ReadStateInternalAsync<T>(string stateName, GrainId grainId, IGrainState<T> grainState)
    {
        var results = await Execute(
            () => Session!.ExecuteAsync(_readStatement!.Bind(new
                {
                    id = GenerateId<T>(stateName, grainId),
                    type = GenerateTypeName<T>(stateName, grainId),
                    name = GenerateStateName<T>(stateName, grainId),
                })));

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
        await Execute(
            () => Session!.ExecuteAsync(_writeStatement!.Bind(new { name, type, id, state, etag, })));
    }

    private async Task ClearStateInternalAsync<T>(string stateName, GrainId grainId, IGrainState<T> grainState)
    {
        var results = await
            Execute(() => Session!.ExecuteAsync(_clearStatement!.Bind(
                new
                {
                    id = GenerateId<T>(stateName, grainId),
                    type = GenerateTypeName<T>(stateName, grainId),
                    name = GenerateStateName<T>(stateName, grainId),
                })));

        if (results.GetAvailableWithoutFetching() != 0)
        {
            grainState.State = default!;
            grainState.ETag = default;
            grainState.RecordExists = default;
        }
    }

    [LoggerMessage(
        EventId = 200,
        EventName = "SingleTable Created",
        Level = LogLevel.Information,
        Message = "Strategy SingleTable '{tableName}' created")]
    private partial void LogTableCreated(string tableName);
}
