//
//  StatefulInjectorTests.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NosSmooth.Core.Client;
using NosSmooth.Core.Commands;
using NosSmooth.Core.Extensions;
using NosSmooth.Core.Packets;
using NosSmooth.Core.Stateful;
using NosSmooth.Core.Tests.Fakes;
using NosSmooth.Core.Tests.Fakes.Commands;
using NosSmooth.Core.Tests.Fakes.Packets;
using NosSmooth.Core.Tests.Packets;
using NosSmooth.Packets.Server.Maps;
using Remora.Results;
using Xunit;

namespace NosSmooth.Core.Tests.Stateful;

/// <summary>
/// Tests injecting stateful entities.
/// </summary>
public class StatefulInjectorTests
{
    /// <summary>
    /// Tests that get entity returns the same instance for the same INostaleClient.
    /// </summary>
    [Fact]
    public void GetEntity_ReturnsSameEntityForSameClient()
    {
        var services = new ServiceCollection()
            .AddSingleton<StatefulInjector>()
            .AddSingleton<StatefulRepository>()
            .AddSingleton<INostaleClient, FakeEmptyNostaleClient>()
            .BuildServiceProvider();
        var injector = new StatefulInjector(new StatefulRepository());
        var client = services.GetRequiredService<INostaleClient>();
        injector.Client = client;
        var entity = injector.GetEntity(services, typeof(FakeEntity));
        var entity2 = injector.GetEntity(services, typeof(FakeEntity));
        Assert.Equal(entity, entity2);
    }

    /// <summary>
    /// Tests that get entity returns different instance for different INostaleClient.
    /// </summary>
    [Fact]
    public void GetEntity_ReturnsDifferentEntityForDifferentClient()
    {
        var services = new ServiceCollection()
            .AddSingleton<StatefulInjector>()
            .AddSingleton<StatefulRepository>()
            .BuildServiceProvider();
        var repository = new StatefulRepository();
        var injector = new StatefulInjector(repository);
        var injector2 = new StatefulInjector(repository);
        var client = new FakeEmptyNostaleClient();
        var client2 = new FakeEmptyNostaleClient();
        injector.Client = client;
        injector2.Client = client2;
        var entity = injector.GetEntity(services, typeof(FakeEntity));
        var entity2 = injector2.GetEntity(services, typeof(FakeEntity));
        Assert.NotEqual(entity, entity2);
    }

    /// <summary>
    /// Tests that extension methods for service provider work correctly with injectable entities, correctly adding pre event to command processor.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    public async Task CommandProcessor_PreEvent_InjectsClient()
    {
        var client1 = new FakeEmptyNostaleClient();
        var client2 = new FakeEmptyNostaleClient();
        FakeEntity? entity1 = null;
        FakeEntity? entity2 = null;
        var services =
            new ServiceCollection()
            .AddSingleton<INostaleClient, FakeEmptyNostaleClient>()
            .AddStatefulInjector()
            .AddStatefulEntity<FakeEntity>()
            .AddSingleton<CommandProcessor>()
            .AddScoped<ICommandHandler<FakeCommand>>
                (p =>
                    {
                        var client = p.GetRequiredService<INostaleClient>();
                        var entity = p.GetRequiredService<FakeEntity>();
                        return new FakeCommandHandler((c) =>
                            {
                                if (c.Input == "1")
                                {
                                    Assert.Equal(client1, client);
                                    entity1 = entity;
                                }
                                else
                                {
                                    Assert.Equal(client2, client);
                                    entity2 = entity;
                                }
                                return Result.FromSuccess();
                            }
                        );
                    }
                )
            .BuildServiceProvider();

        var processor = services.GetRequiredService<CommandProcessor>();

        Assert.True((await processor.ProcessCommand(client1, new FakeCommand("1"), default)).IsSuccess);
        Assert.True((await processor.ProcessCommand(client2, new FakeCommand("2"), default)).IsSuccess);
        Assert.NotNull(entity1);
        Assert.NotNull(entity2);
        Assert.NotEqual(entity1, entity2);
    }

    /// <summary>
    /// Tests that extension methods for service provider work correctly with injectable entities, correctly adding pre event to packet handler.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    public async Task PacketHandler_PreEvent_InjectsClient()
    {
        var client1 = new FakeEmptyNostaleClient();
        var client2 = new FakeEmptyNostaleClient();
        FakeEntity? entity1 = null;
        FakeEntity? entity2 = null;
        var services =
            new ServiceCollection()
                .AddSingleton<INostaleClient, FakeEmptyNostaleClient>()
                .AddStatefulInjector()
                .AddStatefulEntity<FakeEntity>()
                .AddSingleton<CommandProcessor>()
                .AddSingleton<PacketHandler>()
                .AddScoped<IPacketResponder<FakePacket>>
                (p =>
                    {
                        var client = p.GetRequiredService<INostaleClient>();
                        var entity = p.GetRequiredService<FakeEntity>();
                        return new FakePacketResponder<FakePacket>
                        ((c) =>
                            {
                                if (c.Packet.Input == "1")
                                {
                                    Assert.Equal(client1, client);
                                    entity1 = entity;
                                }
                                else
                                {
                                    Assert.Equal(client2, client);
                                    entity2 = entity;
                                }
                                return Result.FromSuccess();
                            }
                        );
                    }
                )
                .BuildServiceProvider();

        var handler = services.GetRequiredService<PacketHandler>();

        Assert.True((await handler.HandleReceivedPacketAsync(client1, new FakePacket("1"), "fake 1")).IsSuccess);
        Assert.True((await handler.HandleReceivedPacketAsync(client2, new FakePacket("2"), "fake 2")).IsSuccess);
        Assert.NotNull(entity1);
        Assert.NotNull(entity2);
        Assert.NotEqual(entity1, entity2);
    }
}