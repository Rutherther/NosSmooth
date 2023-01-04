//
//  PacketHandlerTests.Events.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NosSmooth.Core.Commands;
using NosSmooth.Core.Packets;
using NosSmooth.Core.Tests.Fakes;
using NosSmooth.Core.Tests.Fakes.Commands;
using NosSmooth.Core.Tests.Fakes.Packets;
using NosSmooth.Core.Tests.Fakes.Packets.Events;
using NosSmooth.PacketSerializer.Abstractions.Attributes;
using Remora.Results;
using Xunit;

namespace NosSmooth.Core.Tests.Packets;

/// <summary>
/// Tests <see cref="PacketHandler"/>.
/// </summary>
public class PacketHandlerTestsEvents
{
    /// <summary>
    /// Tests that the handle packet will call the pre event.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Fact]
    public async Task HandlePacket_CallsPreEvent()
    {
        var called = false;
        var client = new FakeEmptyNostaleClient();
        var provider = new ServiceCollection()
            .AddSingleton<PacketHandler>()
            .AddScoped<IPreExecutionEvent>
            (
                _ => new PacketEvent
                (
                    (_, _, p, _) =>
                    {
                        return Result.FromSuccess();
                    },
                    (_, _, _, _, _) => throw new NotImplementedException()
                )
            )
            .AddScoped<IPacketResponder<FakePacket>>
            (
                _ => new FakePacketResponder<FakePacket>
                (
                    args =>
                    {
                        called = true;
                        return Result.FromSuccess();
                    }
                )
            )
            .BuildServiceProvider();

        var result = await provider.GetRequiredService<PacketHandler>().HandlePacketAsync
            (client, PacketSource.Client, new FakePacket("a"), "fake a");
        Assert.True(result.IsSuccess);
        Assert.True(called);
    }

    // TODO: calls pre event
    // TODO: returns pre event error

    // TODO: calls post event
    // TODO: passes correct result to post event
}