//
//  App.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NosSmooth.Core.Client;
using NosSmooth.Core.Extensions;
using NosSmooth.Data.NOSFiles;
using NosSmooth.PacketSerializer.Extensions;
using NosSmooth.PacketSerializer.Packets;
using Remora.Results;

namespace FileClient;

/// <summary>
/// The application.
/// </summary>
public class App : BackgroundService
{
    private readonly INostaleClient _client;
    private readonly IPacketTypesRepository _packetRepository;
    private readonly NostaleDataFilesManager _filesManager;
    private readonly ILogger<App> _logger;
    private readonly IHostLifetime _lifetime;

    /// <summary>
    /// Initializes a new instance of the <see cref="App"/> class.
    /// </summary>
    /// <param name="client">The client.</param>
    /// <param name="packetRepository">The packet repository.</param>
    /// <param name="filesManager">The file manager.</param>
    /// <param name="logger">The logger.</param>
    /// <param name="lifetime">The lifetime.</param>
    public App
    (
        INostaleClient client,
        IPacketTypesRepository packetRepository,
        NostaleDataFilesManager filesManager,
        ILogger<App> logger,
        IHostLifetime lifetime
    )
    {
        _client = client;
        _packetRepository = packetRepository;
        _filesManager = filesManager;
        _logger = logger;
        _lifetime = lifetime;
    }

    /// <inheritdoc />
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var packetResult = _packetRepository.AddDefaultPackets();
        if (!packetResult.IsSuccess)
        {
            _logger.LogResultError(packetResult);
            return;
        }

        var filesResult = _filesManager.Initialize();
        if (!filesResult.IsSuccess)
        {
            _logger.LogResultError(filesResult);
            return;
        }

        var runResult = await _client.RunAsync(stoppingToken);
        if (!runResult.IsSuccess)
        {
            _logger.LogResultError(runResult);
        }

        await _lifetime.StopAsync(default);
    }
}