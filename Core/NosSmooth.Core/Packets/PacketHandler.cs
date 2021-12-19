using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NosCore.Packets.Interfaces;
using Remora.Results;

namespace NosSmooth.Core.Packets;

/// <inheritdoc />
public class PacketHandler : IPacketHandler
{
    private readonly IServiceProvider _provider;

    public PacketHandler(IServiceProvider provider)
    {
        _provider = provider;
    }

    /// <inheritdoc />
    public Task<Result> HandleReceivedPacketAsync(IPacket packet, CancellationToken ct) => HandlePacketAsync(packet, ct);

    /// <inheritdoc />
    public Task<Result> HandleSentPacketAsync(IPacket packet, CancellationToken ct) => HandlePacketAsync(packet, ct);

    private Task<Result> HandlePacketAsync(IPacket packet, CancellationToken ct = default)
    {
        var processMethod = GetType().GetMethod
        (
            nameof(DispatchResponder),
            BindingFlags.NonPublic | BindingFlags.Instance
        );

        if (processMethod is null)
        {
            throw new InvalidOperationException("Could not find process command generic method in command processor.");
        }

        var boundProcessMethod = processMethod.MakeGenericMethod(packet.GetType());
        return (Task<Result>)boundProcessMethod.Invoke(this, new object[] { packet, ct })!;
    }

    private async Task<Result> DispatchResponder<TPacket>(TPacket packet, CancellationToken ct)
        where TPacket : class, IPacket
    {
        using var scope = _provider.CreateScope();
        var packetResponders = scope.ServiceProvider.GetServices<IPacketResponder<TPacket>>();
        var genericPacketResponders = scope.ServiceProvider.GetServices<IEveryPacketResponder>();

        var tasks = packetResponders.Select(responder => responder.Respond(packet, ct)).ToList();
        tasks.AddRange(genericPacketResponders.Select(responder => responder.Respond(packet, ct)));
        
        var results = await Task.WhenAll(tasks);
        
        var errors = new List<Result>();
        foreach (var result in results)
        {
            if (!result.IsSuccess)
            {
                errors.Add(result);
            }
        }

        return errors.Count switch
        {
            0 => Result.FromSuccess(),
            1 => errors[0],
            _ => new AggregateError(errors.Cast<IResult>().ToList())
        };
    }
}