﻿// Copyright (c) Escendit Ltd. All Rights Reserved.
// Licensed under the MIT. See LICENSE.txt file in the solution root for full license information.

namespace Escendit.Orleans.Persistence.Cassandra.Storage;

using System.Diagnostics;
using System.Runtime.CompilerServices;
using Escendit.Extensions.Hosting.Cassandra;
using global::Cassandra;
using global::Orleans;
using global::Orleans.Runtime;
using global::Orleans.Storage;
using Microsoft.Extensions.Logging;
using Options;

/// <summary>
/// Grain Storage Base.
/// </summary>
internal abstract partial class GrainStorageBase : IGrainStorage, ILifecycleParticipant<ISiloLifecycle>, IDisposable
{
    private readonly string _name;
    private readonly ILogger _logger;
    private readonly ICluster _cluster;
    private readonly CassandraClientOptions _clientOptions;
    private readonly CassandraStorageOptions _storageOptions;

    /// <summary>
    /// Initializes a new instance of the <see cref="GrainStorageBase"/> class.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="logger">The logger.</param>
    /// <param name="cluster">The cluster.</param>
    /// <param name="clientOptions">The client options.</param>
    /// <param name="storageOptions">The storage options.</param>
    protected GrainStorageBase(
        string name,
        ILogger logger,
        ICluster cluster,
        CassandraClientOptions clientOptions,
        CassandraStorageOptions storageOptions)
    {
        _name = name;
        _logger = logger;
        _cluster = cluster;
        _clientOptions = clientOptions;
        _storageOptions = storageOptions;
    }

    /// <summary>
    /// Finalizes an instance of the <see cref="GrainStorageBase"/> class.
    /// </summary>
    ~GrainStorageBase() => Dispose(false);

    /// <summary>
    /// Gets the session.
    /// </summary>
    /// <value>The session.</value>
    protected ISession? Session { get; private set; }

    /// <inheritdoc/>
    public abstract Task ReadStateAsync<T>(string stateName, GrainId grainId, IGrainState<T> grainState);

    /// <inheritdoc/>
    public abstract Task WriteStateAsync<T>(string stateName, GrainId grainId, IGrainState<T> grainState);

    /// <inheritdoc/>
    public abstract Task ClearStateAsync<T>(string stateName, GrainId grainId, IGrainState<T> grainState);

    /// <inheritdoc/>
    public void Participate(ISiloLifecycle observer)
    {
        LogParticipate(_name);
        observer.Subscribe($"{GetType().Name}:{_name}", _storageOptions.InitialStage, Initialize);
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// The init method.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
    protected virtual async Task Initialize(CancellationToken cancellationToken)
    {
        LogInitialize(_name, _clientOptions.DefaultKeyspace!);
        Session = await _cluster
            .ConnectAsync(string.Empty)
            .ConfigureAwait(false);
        Session.CreateKeyspaceIfNotExists(_clientOptions.DefaultKeyspace);
        Session.ChangeKeyspace(_clientOptions.DefaultKeyspace);
        LogConnect(_name, _clientOptions.DefaultKeyspace!);
    }

    /// <summary>
    /// Dispose.
    /// </summary>
    /// <param name="disposing">The disposing.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!disposing)
        {
            return;
        }

        Session?.Dispose();
        _cluster.Shutdown();
        _cluster.Dispose();
    }

    /// <summary>
    /// Generate Type Name.
    /// </summary>
    /// <param name="stateName">The state name.</param>
    /// <param name="grainId">The grain id.</param>
    /// <typeparam name="TGrain">The grain type.</typeparam>
    /// <returns>The string.</returns>
    protected virtual string GenerateTypeName<TGrain>(string stateName, GrainId grainId)
    {
        return typeof(TGrain).FullName ?? typeof(TGrain).Name;
    }

    /// <summary>
    /// Generate State Name.
    /// </summary>
    /// <param name="stateName">The state name.</param>
    /// <param name="grainId">The grain id.</param>
    /// <typeparam name="TGrain">The grain type.</typeparam>
    /// <returns>The string.</returns>
    protected virtual string GenerateStateName<TGrain>(string stateName, GrainId grainId)
    {
        return stateName;
    }

    /// <summary>
    /// Generate Id.
    /// </summary>
    /// <param name="stateName">The state name.</param>
    /// <param name="grainId">The grain id.</param>
    /// <typeparam name="TGrain">The grain type.</typeparam>
    /// <returns>The blob.</returns>
    protected virtual byte[] GenerateId<TGrain>(string stateName, GrainId grainId)
    {
        return grainId.Key.Value.ToArray();
    }

    /// <summary>
    /// Execute.
    /// </summary>
    /// <param name="action">The action.</param>
    /// <param name="actionName">The action name.</param>
    /// <returns>The task.</returns>
    protected Task Execute(Func<Task> action, [CallerMemberName] string actionName = default!)
    {
        ArgumentNullException.ThrowIfNull(action);
        return Execute(async () =>
        {
            await action();
            return true;
        });
    }

    /// <summary>
    /// Execute.
    /// </summary>
    /// <typeparam name="T">The type.</typeparam>
    /// <param name="action">The action.</param>
    /// <param name="actionName">The action name.</param>
    /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
    protected Task<T> Execute<T>(Func<Task<T>> action, [CallerMemberName] string actionName = default!)
    {
        ArgumentNullException.ThrowIfNull(action);
        var returnType = typeof(T);
        var selfType = GetType();
        var returnTypeName = returnType.Name;
        var selfTypeName = selfType.Name;
        if (returnType.GenericTypeArguments.Any())
        {
            var arguments = string.Join(", ", returnType.GenericTypeArguments.ToList().ConvertAll(item => item.Name));
            returnTypeName = $"{returnTypeName}<{arguments}>";
        }

        try
        {
            var stopwatch = Stopwatch.StartNew();
            var result = action();
            result.ContinueWith(
                (_, _) =>
                {
                    stopwatch.Stop();
                    LogExecute(_name, returnTypeName, selfTypeName, actionName, stopwatch.ElapsedMilliseconds);
                },
                TaskContinuationOptions.OnlyOnRanToCompletion,
                TaskScheduler.Default);
            return result;
        }
        catch (Exception ex)
        {
            LogException(_name, ex, ex.Message, returnTypeName, selfTypeName, actionName);
            throw new CassandraStorageException(ex.Message, ex);
        }
    }

    [LoggerMessage(
        EventId = 0,
        EventName = "Participate",
        Level = LogLevel.Debug,
        Message = "Participating {name} in Lifecycle")]
    private partial void LogParticipate(string name);

    [LoggerMessage(
        EventId = 100,
        EventName = "Initialize",
        Level = LogLevel.Information,
        Message = "Initializing {name} to {keyspace}")]
    private partial void LogInitialize(string name, string keyspace);

    [LoggerMessage(
        EventId = 101,
        EventName = "Connect",
        Level = LogLevel.Information,
        Message = "Connected {name} to {keyspace}")]
    private partial void LogConnect(string name, string keyspace);

    [LoggerMessage(
        EventId = 200,
        EventName = "Execution",
        Level = LogLevel.Debug,
        Message = "Executing with client {name} > {returnType} {selfType}.{action} completed in {elapsed}ms")]
    private partial void LogExecute(string name, string returnType, string selfType, string action, long elapsed);

    [LoggerMessage(
        EventId = 500,
        EventName = "Exception",
        Level = LogLevel.Error,
        Message = "Exception with client {name} > {returnType} {selfType}.{action} {message}")]
    private partial void LogException(string name, Exception exception, string message, string returnType, string selfType, string action);
}
