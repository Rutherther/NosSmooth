//
//  PacketFileClient.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Text.RegularExpressions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NosSmooth.Core.Client;
using NosSmooth.Core.Commands;
using NosSmooth.Core.Extensions;
using NosSmooth.Core.Packets;
using NosSmooth.Data.NOSFiles;
using NosSmooth.Data.NOSFiles.Extensions;
using NosSmooth.Game.Extensions;
using NosSmooth.Game.Tests.Helpers;
using NosSmooth.Packets;
using NosSmooth.PacketSerializer;
using NosSmooth.PacketSerializer.Abstractions.Attributes;
using NosSmooth.PacketSerializer.Errors;
using NosSmooth.PacketSerializer.Extensions;
using NosSmooth.PacketSerializer.Packets;
using Remora.Results;
using Xunit.Abstractions;

namespace NosSmooth.Game.Tests;

/// <summary>
/// A client used for tests. Supports loading just part of a file with packets.
/// </summary>
public class PacketFileClient : BaseNostaleClient, IDisposable
{
    private const string LineRegex = ".*\\[(Recv|Send)\\]\t(.*)";
    private const string LabelRegex = "##(.*)";

    // TODO: make this class cleaner

    private readonly FileStream _stream;
    private readonly StreamReader _reader;
    private readonly IPacketSerializer _packetSerializer;
    private readonly PacketHandler _packetHandler;
    private readonly ILogger<PacketFileClient> _logger;
    private string? _nextLabel;
    private bool _skip;
    private bool _readToLabel;

    /// <summary>
    /// Builds a file client for the given test.
    /// </summary>
    /// <param name="testName">The name of the test.</param>
    /// <param name="testOutputHelper">The output helper to output logs to.</param>
    /// <typeparam name="TTest">The test type.</typeparam>
    /// <returns>A file client and the associated game.</returns>
    public static (PacketFileClient Client, Game Game) CreateFor<TTest>(string testName, ITestOutputHelper testOutputHelper)
    {
        var services = new ServiceCollection()
            .AddLogging(b => b.AddProvider(new XUnitLoggerProvider(testOutputHelper)))
            .AddNostaleCore()
            .AddNostaleGame()
            .AddSingleton<PacketFileClient>(p => CreateFor<TTest>(p, testName))
            .AddSingleton<INostaleClient>(p => p.GetRequiredService<PacketFileClient>())
            .AddNostaleDataFiles()
            .BuildServiceProvider();

        services.GetRequiredService<IPacketTypesRepository>().AddDefaultPackets();
        if (!services.GetRequiredService<NostaleDataFilesManager>().Initialize().IsSuccess)
        {
            throw new Exception("Data not initialized correctly.");
        }

        return (services.GetRequiredService<PacketFileClient>(), services.GetRequiredService<Game>());
    }

    /// <summary>
    /// Create a file client for the given test.
    /// </summary>
    /// <param name="services">The services provider.</param>
    /// <param name="testName">The name of the test.</param>
    /// <typeparam name="TTest">The test class.</typeparam>
    /// <returns>A client.</returns>
    public static PacketFileClient CreateFor<TTest>(IServiceProvider services, string testName)
    {
        var prefix = "NosSmooth.Game.Tests.";
        var name = typeof(TTest).FullName!.Substring(prefix.Length).Replace("Tests", string.Empty);

        var splitted = name.Split('.');
        var path = "Packets/";

        foreach (var entry in splitted)
        {
            path += entry + "/";
        }

        path += testName + ".plog";

        return Create
        (
            services,
            path
        );
    }

    /// <summary>
    /// Create an instance of PacketFileClient for the given file.
    /// </summary>
    /// <param name="services">The services provider.</param>
    /// <param name="fileName">The file name.</param>
    /// <returns>A client.</returns>
    public static PacketFileClient Create(IServiceProvider services, string fileName)
    {
        return (PacketFileClient)ActivatorUtilities.CreateInstance
            (services, typeof(PacketFileClient), new[] { File.OpenRead(fileName) });
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PacketFileClient"/> class.
    /// </summary>
    /// <param name="stream">The file stream.</param>
    /// <param name="packetSerializer">The packet serializer.</param>
    /// <param name="commandProcessor">The command processor.</param>
    /// <param name="packetHandler">The packet handler.</param>
    /// <param name="logger">The logger.</param>
    public PacketFileClient
    (
        FileStream stream,
        IPacketSerializer packetSerializer,
        CommandProcessor commandProcessor,
        PacketHandler packetHandler,
        ILogger<PacketFileClient> logger
    )
        : base(commandProcessor, packetSerializer)
    {
        _stream = stream;
        _reader = new StreamReader(_stream);
        _packetSerializer = packetSerializer;
        _packetHandler = packetHandler;
        _logger = logger;
    }

    /// <summary>
    /// Start executing until the given label is hit.
    /// </summary>
    /// <param name="label">The label to hit.</param>
    /// <returns>An asynchronous operation.</returns>
    public async Task ExecuteUntilLabelAsync(string label)
    {
        _readToLabel = false;
        _nextLabel = label;
        await RunAsync();

        if (!_readToLabel)
        {
            throw new Exception($"Label {label} not found.");
        }
    }

    /// <summary>
    /// Start executing until the end of the file.
    /// </summary>
    /// <returns>An asynchronous operation.</returns>
    public Task ExecuteToEnd()
    {
        _nextLabel = null;
        return RunAsync();
    }

    /// <summary>
    /// Skip cursor until the given label is hit.
    /// </summary>
    /// <param name="label">The label to hit.</param>
    /// <returns>An asynchronous operation.</returns>
    public async Task SkipUntilLabelAsync(string label)
    {
        try
        {
            _readToLabel = false;
            _nextLabel = label;
            _skip = true;
            await RunAsync();
        }
        finally
        {
            _skip = false;
        }

        if (!_readToLabel)
        {
            throw new Exception($"Label {label} not found.");
        }
    }

    /// <inheritdoc />
    public override async Task<Result> RunAsync(CancellationToken stopRequested = default)
    {
        var packetRegex = new Regex(LineRegex);
        var labelRegex = new Regex(LabelRegex);
        while (!_reader.EndOfStream)
        {
            stopRequested.ThrowIfCancellationRequested();
            var line = await _reader.ReadLineAsync(stopRequested);
            if (string.IsNullOrEmpty(line))
            {
                continue;
            }

            var labelMatch = labelRegex.Match(line);
            if (labelMatch.Success)
            {
                var label = labelMatch.Groups[1].Value;
                if (label == _nextLabel)
                {
                    _readToLabel = true;
                    break;
                }

                continue;
            }

            if (_skip)
            {
                continue;
            }

            var packetMatch = packetRegex.Match(line);
            if (!packetMatch.Success)
            {
                _logger.LogWarning($"Could not find match on line {line}");
                continue;
            }

            var type = packetMatch.Groups[1].Value;
            var packetStr = packetMatch.Groups[2].Value;

            var source = type == "Recv" ? PacketSource.Server : PacketSource.Client;
            var packet = CreatePacket(packetStr, source);
            Result result = await _packetHandler.HandlePacketAsync
            (
                this,
                source,
                packet,
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
            CreatePacket(packetString, PacketSource.Client),
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
            CreatePacket(packetString, PacketSource.Server),
            packetString,
            ct
        );
    }

    private IPacket CreatePacket(string packetStr, PacketSource source)
    {
        var packetResult = _packetSerializer.Deserialize(packetStr, source);
        if (!packetResult.IsSuccess)
        {
            if (packetResult.Error is PacketConverterNotFoundError err)
            {
                return new UnresolvedPacket(err.Header, packetStr);
            }

            return new ParsingFailedPacket(packetResult, packetStr);
        }

        return packetResult.Entity;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        _stream.Dispose();
        _reader.Dispose();
    }
}