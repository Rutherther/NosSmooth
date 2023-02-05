//
//  Client.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using NosSmooth.Core.Client;
using NosSmooth.Core.Commands;
using NosSmooth.Core.Extensions;
using NosSmooth.Core.Packets;
using NosSmooth.Packets;
using NosSmooth.PacketSerializer;
using NosSmooth.PacketSerializer.Abstractions.Attributes;
using NosSmooth.PacketSerializer.Errors;
using Remora.Results;

namespace FileClient;

/// <summary>
/// A NosTale client using stream to read lines.
/// </summary>
public class Client : BaseNostaleClient
{
    private const string LineRegex = ".*\\[(Recv|Send)\\]\t(.*)";
    private readonly IPacketHandler _packetHandler;
    private readonly ILogger<Client> _logger;
    private readonly Stream _stream;

    /// <summary>
    /// Initializes a new instance of the <see cref="Client"/> class.
    /// </summary>
    /// <param name="stream">The stream with packets.</param>
    /// <param name="packetHandler">The packet handler.</param>
    /// <param name="commandProcessor">The command processor.</param>
    /// <param name="logger">The logger.</param>
    public Client
    (
        Stream stream,
        IPacketHandler packetHandler,
        CommandProcessor commandProcessor,
        ILogger<Client> logger
    )
        : base(commandProcessor)
    {
        _stream = stream;
        _packetHandler = packetHandler;
        _logger = logger;
    }

    /// <inheritdoc />
    public override async Task<Result> RunAsync(CancellationToken stopRequested = default)
    {
        using var reader = new StreamReader(_stream);
        var regex = new Regex(LineRegex);
        while (!reader.EndOfStream)
        {
            stopRequested.ThrowIfCancellationRequested();
            var line = await reader.ReadLineAsync();
            if (string.IsNullOrEmpty(line))
            {
                continue;
            }

            var match = regex.Match(line);
            if (!match.Success)
            {
                _logger.LogError("Could not find match on line {Line}", line);
                continue;
            }

            var type = match.Groups[1].Value;
            var packetStr = match.Groups[2].Value;

            var source = type == "Recv" ? PacketSource.Server : PacketSource.Client;
            Result result = await _packetHandler.HandlePacketAsync
            (
                this,
                source,
                packetStr,
                stopRequested
            );
            if (!result.IsSuccess)
            {
                _logger.LogResultError(result);
            }
        }

        return Result.FromSuccess();
    }

    /// <inheritdoc/>
    public override Task<Result> SendPacketAsync(string packetString, CancellationToken ct = default)
    {
        return _packetHandler.HandlePacketAsync
        (
            this,
            PacketSource.Client,
            packetString,
            ct
        );
    }

    /// <inheritdoc/>
    public override Task<Result> ReceivePacketAsync(string packetString, CancellationToken ct = default)
    {
        return _packetHandler.HandlePacketAsync
        (
            this,
            PacketSource.Server,
            packetString,
            ct
        );
    }
}