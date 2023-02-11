//
//  ProcessTcpManager.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics;
using Microsoft.Extensions.Options;

namespace NosSmooth.Pcap;

/// <summary>
/// A manager containing tcp connections, allowing notifications
/// to <see cref="PcapNostaleClient"/> to know about any new connections.
/// </summary>
public class ProcessTcpManager
{
    private readonly PcapNostaleOptions _options;

    private readonly SemaphoreSlim _semaphore;
    private readonly List<int> _processes;
    private DateTimeOffset _lastRefresh;
    private IReadOnlyDictionary<int, List<TcpConnection>> _connections;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProcessTcpManager"/> class.
    /// </summary>
    /// <param name="options">The options.</param>
    public ProcessTcpManager(IOptions<PcapNostaleOptions> options)
    {
        _options = options.Value;
        _lastRefresh = DateTimeOffset.MinValue;
        _semaphore = new SemaphoreSlim(1, 1);
        _processes = new List<int>();
        _connections = new Dictionary<int, List<TcpConnection>>();
    }

    /// <summary>
    /// Register the given process to refreshing list to allow calling <see cref="GetConnectionsAsync"/>
    /// with that process.
    /// </summary>
    /// <param name="processId">The id of the process to register.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task RegisterProcess(int processId)
    {
        await _semaphore.WaitAsync();
        try
        {
            _processes.Add(processId);
        }
        finally
        {
            _semaphore.Release();
        }
    }

    /// <summary>
    /// Unregister the given process from refreshing list, <see cref="GetConnectionsAsync"/> won't
    /// work for that process anymore.
    /// </summary>
    /// <param name="processId">The process to unregister.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task UnregisterProcess(int processId)
    {
        await _semaphore.WaitAsync();
        try
        {
            _processes.Remove(processId);
        }
        finally
        {
            _semaphore.Release();
        }
    }

    /// <summary>
    /// Get connections established by the given process.
    /// </summary>
    /// <remarks>
    /// Works only for processes registered using <see cref="RegisterProcess"/>.
    /// </remarks>
    /// <param name="processId">The id of process to retrieve connections for.</param>
    /// <returns>The list of process connections.</returns>
    public async Task<IReadOnlyList<TcpConnection>> GetConnectionsAsync(int processId)
    {
        await Refresh();

        if (!_connections.ContainsKey(processId))
        {
            return Array.Empty<TcpConnection>();
        }

        return _connections[processId];
    }

    private async Task Refresh()
    {
        if (_lastRefresh.AddMilliseconds(_options.ProcessRefreshInterval) >= DateTimeOffset.Now)
        {
            return;
        }

        _lastRefresh = DateTimeOffset.Now;
        if (_processes.Count == 0)
        {
            if (_connections.Count > 0)
            {
                _connections = new Dictionary<int, List<TcpConnection>>();
            }
        }

        await _semaphore.WaitAsync();
        _connections = TcpConnectionHelper.GetConnections(_processes);
        _semaphore.Release();
    }
}