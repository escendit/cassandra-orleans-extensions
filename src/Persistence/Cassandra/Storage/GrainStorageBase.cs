// Copyright (c) Escendit Ltd. All Rights Reserved.
// Licensed under the MIT. See LICENSE.txt file in the solution root for full license information.

namespace Escendit.Orleans.Persistence.Cassandra.Storage;

using System.Runtime.CompilerServices;
using Escendit.Extensions.Hosting.Cassandra;
using global::Cassandra;
using global::Orleans;
using global::Orleans.Runtime;
using global::Orleans.Storage;
using Microsoft.Extensions.Logging;

/// <summary>
/// Grain Storage Base.
/// </summary>
public abstract partial class GrainStorageBase : IGrainStorage, ILifecycleParticipant<ISiloLifecycle>, IDisposable
{
    private readonly string _name;
    private readonly ILogger _logger;
    private readonly ICluster _cluster;
    private readonly CassandraClientOptions _options;

    /// <summary>
    /// Initializes a new instance of the <see cref="GrainStorageBase"/> class.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="logger">The logger.</param>
    /// <param name="cluster">The cluster.</param>
    /// <param name="options">The client options.</param>
    protected GrainStorageBase(
        string name,
        ILogger logger,
        ICluster cluster,
        CassandraClientOptions options)
    {
        _name = name;
        _logger = logger;
        _cluster = cluster;
        _options = options;
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
        observer.Subscribe($"{GetType().Name}:{_name}", ServiceLifecycleStage.ApplicationServices, Initialize);
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
        LogInitialize(_name, _options.DefaultKeyspace!);
        Session = await _cluster.ConnectAsync(string.Empty);
        Session.CreateKeyspaceIfNotExists(_options.DefaultKeyspace);
        Session.ChangeKeyspace(_options.DefaultKeyspace);
        LogConnect(_name, _options.DefaultKeyspace!);
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

        try
        {
            LogExecute(_name, typeof(T).FullName!, actionName);
            return action();
        }
        catch (Exception ex)
        {
            LogException(_name, ex, ex.Message, nameof(SingleTableGrainStorage), actionName);
            throw;
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
        Message = "Executing {name}#{type}.{action}")]
    private partial void LogExecute(string name, string type, string action);

    [LoggerMessage(
        EventId = 500,
        EventName = "Exception",
        Level = LogLevel.Error,
        Message = "{name}#{type}.{action} {message}")]
    private partial void LogException(string name, Exception exception, string message, string type, string action);
}
