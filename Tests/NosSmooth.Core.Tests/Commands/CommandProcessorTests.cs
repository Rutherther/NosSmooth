//
//  CommandProcessorTests.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NosSmooth.Core.Commands;
using NosSmooth.Core.Errors;
using NosSmooth.Core.Extensions;
using NosSmooth.Core.Tests.Fakes;
using NosSmooth.Core.Tests.Fakes.Commands;
using NosSmooth.Core.Tests.Fakes.Commands.Events;
using Remora.Results;
using Xunit;

namespace NosSmooth.Core.Tests.Commands;

/// <summary>
/// Test for <see cref="CommandProcessor"/>.
/// </summary>
public class CommandProcessorTests
{
    /// <summary>
    /// Tests that unknown not registered command should return a <see cref="CommandHandlerNotFound"/>.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Fact]
    public async Task ProcessCommand_UnknownCommand_ShouldReturnError()
    {
        var provider = new ServiceCollection()
            .AddSingleton<CommandProcessor>()
            .BuildServiceProvider();

        var processResult = await provider.GetRequiredService<CommandProcessor>().ProcessCommand
            (new FakeEmptyNostaleClient(), new FakeCommand("asdf"));
        Assert.False(processResult.IsSuccess);
        Assert.IsType<CommandHandlerNotFound>(processResult.Error);
    }

    /// <summary>
    /// Tests that known command has its handler called.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    public async Task ProcessCommand_KnownCommand_ShouldCallHandler()
    {
        bool called = false;
        var fakeCommand = new FakeCommand("asdf");
        var provider = new ServiceCollection()
            .AddSingleton<CommandProcessor>()
            .AddScoped<ICommandHandler<FakeCommand>, FakeCommandHandler>
            (
                _ => new FakeCommandHandler
                (
                    fc =>
                    {
                        Assert.Equal(fakeCommand, fc);
                        called = true;
                        return Result.FromSuccess();
                    }
                )
            )
            .BuildServiceProvider();

        var processResult = await provider.GetRequiredService<CommandProcessor>().ProcessCommand
            (new FakeEmptyNostaleClient(), fakeCommand);
        Assert.True(processResult.IsSuccess);
        Assert.True(called);
    }

    /// <summary>
    /// Tests that if there are pre events they will be called.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    public async Task ProcessCommand_HavingPreEvents_ShouldCallPreEvents()
    {
        bool called = false;
        var fakeCommand = new FakeCommand("asdf");
        var provider = new ServiceCollection()
            .AddSingleton<CommandProcessor>()
            .AddPreCommandExecutionEvent<SuccessfulCommandEvent>()
            .AddScoped<IPreCommandExecutionEvent>
            (
                _ => new CommandEvent<FakeCommand>
                (
                    c =>
                    {
                        Assert.Equal(fakeCommand, c);
                        called = true;
                        return Result.FromSuccess();
                    },
                    (_, _) => throw new NotImplementedException()
                )
            )
            .AddScoped<ICommandHandler<FakeCommand>, FakeCommandHandler>
            (
                _ => new FakeCommandHandler
                (
                    fc => Result.FromSuccess()
                )
            )
            .BuildServiceProvider();

        var processResult = await provider.GetRequiredService<CommandProcessor>().ProcessCommand
            (new FakeEmptyNostaleClient(), fakeCommand);
        Assert.True(processResult.IsSuccess);
        Assert.True(called);
    }

    /// <summary>
    /// Tests that if there are pre events that return an error, the handler of the command won't be called.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    public async Task ProcessCommand_HavingErrorfulPreEvents_ShouldNotCallHandler()
    {
        bool called = false;
        var fakeCommand = new FakeCommand("asdf");
        var provider = new ServiceCollection()
            .AddSingleton<CommandProcessor>()
            .AddPreCommandExecutionEvent<ErrorCommandEvent>()
            .AddScoped<ICommandHandler<FakeCommand>, FakeCommandHandler>
            (
                _ => new FakeCommandHandler
                (
                    fc =>
                    {
                        called = true;
                        return Result.FromSuccess();
                    }
                )
            )
            .BuildServiceProvider();

        var processResult = await provider.GetRequiredService<CommandProcessor>().ProcessCommand
            (new FakeEmptyNostaleClient(), fakeCommand);
        Assert.False(processResult.IsSuccess);
        Assert.False(called);
    }

    /// <summary>
    /// Tests that if there are pre events that return successful result, the handler should be called.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    public async Task ProcessCommand_HavingSuccessfulPreEvents_ShouldCallHandler()
    {
        bool called = false;
        var fakeCommand = new FakeCommand("asdf");
        var provider = new ServiceCollection()
            .AddSingleton<CommandProcessor>()
            .AddPreCommandExecutionEvent<SuccessfulCommandEvent>()
            .AddScoped<ICommandHandler<FakeCommand>, FakeCommandHandler>
            (
                _ => new FakeCommandHandler
                (
                    fc =>
                    {
                        Assert.Equal(fakeCommand, fc);
                        called = true;
                        return Result.FromSuccess();
                    }
                )
            )
            .BuildServiceProvider();

        var processResult = await provider.GetRequiredService<CommandProcessor>().ProcessCommand
            (new FakeEmptyNostaleClient(), fakeCommand);
        Assert.True(processResult.IsSuccess);
        Assert.True(called);
    }

    /// <summary>
    /// Tests if there are post events they will be called.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    public async Task ProcessCommand_HavingPostEvents_ShouldCallPostEvents()
    {
        bool called = false;
        var fakeCommand = new FakeCommand("asdf");
        var provider = new ServiceCollection()
            .AddSingleton<CommandProcessor>()
            .AddScoped<IPostCommandExecutionEvent>
            (
                _ => new CommandEvent<FakeCommand>
                (
                    _ => throw new NotImplementedException(),
                    (fc, res) =>
                    {
                        Assert.Equal(fakeCommand, fc);
                        Assert.True(res.IsSuccess);
                        called = true;
                        return Result.FromSuccess();
                    }
                )
            )
            .AddScoped<ICommandHandler<FakeCommand>, FakeCommandHandler>
            (
                _ => new FakeCommandHandler
                (
                    fc => { return Result.FromSuccess(); }
                )
            )
            .BuildServiceProvider();

        var processResult = await provider.GetRequiredService<CommandProcessor>().ProcessCommand
            (new FakeEmptyNostaleClient(), fakeCommand);
        Assert.True(processResult.IsSuccess);
        Assert.True(called);
    }

    /// <summary>
    /// Tests if there are post events, the successful result from the handler should be passed to them.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    public async Task ProcessCommand_HavingPostEvents_ShouldPassSuccessfulResultToPostEvents()
    {
        bool called = false;
        var fakeCommand = new FakeCommand("asdf");
        var provider = new ServiceCollection()
            .AddSingleton<CommandProcessor>()
            .AddScoped<IPostCommandExecutionEvent>
            (
                _ => new CommandEvent<FakeCommand>
                (
                    _ => throw new NotImplementedException(),
                    (fc, res) =>
                    {
                        Assert.Equal(fakeCommand, fc);
                        Assert.True(res.IsSuccess);
                        called = true;
                        return Result.FromSuccess();
                    }
                )
            )
            .AddScoped<ICommandHandler<FakeCommand>, FakeCommandHandler>
            (
                _ => new FakeCommandHandler
                (
                    fc => { return Result.FromSuccess(); }
                )
            )
            .BuildServiceProvider();

        var processResult = await provider.GetRequiredService<CommandProcessor>().ProcessCommand
            (new FakeEmptyNostaleClient(), fakeCommand);
        Assert.True(processResult.IsSuccess);
        Assert.True(called);
    }

    /// <summary>
    /// Tests if there are post events, the error from the handler should be passed to them.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    public async Task ProcessCommand_HavingPostEvents_ShouldPassErrorfulResultToPostEvents()
    {
        bool called = false;
        var fakeCommand = new FakeCommand("asdf");
        var provider = new ServiceCollection()
            .AddSingleton<CommandProcessor>()
            .AddScoped<IPostCommandExecutionEvent>
            (
                _ => new CommandEvent<FakeCommand>
                (
                    _ => throw new NotImplementedException(),
                    (fc, res) =>
                    {
                        Assert.Equal(fakeCommand, fc);
                        Assert.False(res.IsSuccess);
                        Assert.IsType<GenericError>(res.Error);
                        called = true;
                        return Result.FromSuccess();
                    }
                )
            )
            .AddScoped<ICommandHandler<FakeCommand>, FakeCommandHandler>
            (
                _ => new FakeCommandHandler
                (
                    fc => { return new FakeError("Error"); }
                )
            )
            .BuildServiceProvider();

        var processResult = await provider.GetRequiredService<CommandProcessor>().ProcessCommand
            (new FakeEmptyNostaleClient(), fakeCommand);
        Assert.False(processResult.IsSuccess);
        Assert.True(called);
    }

    /// <summary>
    /// Tests that error from post events is returned.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    public async Task ProcessCommand_HavingErrorfulPostEvents_ShouldReturnPostExecutionError()
    {
        var fakeCommand = new FakeCommand("asdf");
        var error = new FakeError("Error");
        var provider = new ServiceCollection()
            .AddSingleton<CommandProcessor>()
            .AddScoped<IPostCommandExecutionEvent>
            (
                _ => new CommandEvent<FakeCommand>
                (
                    _ => throw new NotImplementedException(),
                    (fc, _) =>
                    {
                        Assert.Equal(fakeCommand, fc);
                        return error;
                    }
                )
            )
            .AddScoped<ICommandHandler<FakeCommand>, FakeCommandHandler>
            (
                _ => new FakeCommandHandler
                (
                    fc => Result.FromSuccess()
                )
            )
            .BuildServiceProvider();

        var processResult = await provider.GetRequiredService<CommandProcessor>().ProcessCommand
            (new FakeEmptyNostaleClient(), fakeCommand);
        Assert.False(processResult.IsSuccess);
        Assert.Equal(error, processResult.Error);
    }

    /// <summary>
    /// Tests that error from pre events is returned.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    public async Task ProcessCommand_HavingErrorfulPreEvents_ShouldReturnPreExecutionError()
    {
        var fakeCommand = new FakeCommand("asdf");
        var error = new FakeError("Error");
        var provider = new ServiceCollection()
            .AddSingleton<CommandProcessor>()
            .AddScoped<IPreCommandExecutionEvent>
            (
                _ => new CommandEvent<FakeCommand>
                (
                    (fc) =>
                    {
                        Assert.Equal(fakeCommand, fc);
                        return error;
                    },
                    (_, _) => throw new NotImplementedException()
                )
            )
            .AddScoped<ICommandHandler<FakeCommand>, FakeCommandHandler>
            (
                _ => new FakeCommandHandler
                (
                    fc => Result.FromSuccess()
                )
            )
            .BuildServiceProvider();

        var processResult = await provider.GetRequiredService<CommandProcessor>().ProcessCommand
            (new FakeEmptyNostaleClient(), fakeCommand);
        Assert.False(processResult.IsSuccess);
        Assert.Equal(error, processResult.Error);
    }

    /// <summary>
    /// Tests that error from post event and handler is returned.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    public async Task ProcessCommand_HavingErrorfulPostEventsAndHandler_ShouldReturnHandlerAndPostExecutionError()
    {
        var fakeCommand = new FakeCommand("asdf");
        var error1 = new FakeError();
        var error2 = new FakeError();
        var provider = new ServiceCollection()
            .AddSingleton<CommandProcessor>()
            .AddScoped<IPostCommandExecutionEvent>
            (
                _ => new CommandEvent<FakeCommand>
                (
                    _ => throw new NotImplementedException(),
                    (fc, _) =>
                    {
                        Assert.Equal(fakeCommand, fc);
                        return error1;
                    }
                )
            )
            .AddScoped<ICommandHandler<FakeCommand>, FakeCommandHandler>
            (
                _ => new FakeCommandHandler
                (
                    fc => error2
                )
            )
            .BuildServiceProvider();

        var processResult = await provider.GetRequiredService<CommandProcessor>().ProcessCommand
            (new FakeEmptyNostaleClient(), fakeCommand);
        Assert.False(processResult.IsSuccess);
        Assert.IsType<AggregateError>(processResult.Error);
        var aggregateError = processResult.Error as AggregateError;
        Assert.NotNull(aggregateError);
        if (aggregateError is not null)
        {
            Assert.True(aggregateError.Errors.Any(x => x.Error == error1));

            Assert.True(aggregateError.Errors.Any(x => x.Error == error2));
            Assert.Equal(2, aggregateError.Errors.Count);
        }
    }

    /// <summary>
    /// Tests that exceptions are handled.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    public async Task ProcessCommand_HavingExceptionInHandler_ShouldNotThrow()
    {
        var fakeCommand = new FakeCommand("asdf");
        var provider = new ServiceCollection()
            .AddSingleton<CommandProcessor>()
            .AddScoped<IPostCommandExecutionEvent>
            (
                _ => new CommandEvent<FakeCommand>
                (
                    _ => throw new Exception(),
                    (_, _) => throw new Exception()
                )
            )
            .AddScoped<ICommandHandler<FakeCommand>, FakeCommandHandler>
            (
                _ => new FakeCommandHandler
                (
                    fc => throw new Exception()
                )
            )
            .BuildServiceProvider();

        await provider.GetRequiredService<CommandProcessor>().ProcessCommand
            (new FakeEmptyNostaleClient(), fakeCommand);
    }

    /// <summary>
    /// Tests that exceptions are handled.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    public async Task ProcessCommand_HavingExceptionInPreEvent_ShouldNotThrow()
    {
        var fakeCommand = new FakeCommand("asdf");
        var provider = new ServiceCollection()
            .AddSingleton<CommandProcessor>()
            .AddScoped<IPreCommandExecutionEvent>
            (
                _ => new CommandEvent<FakeCommand>
                (
                    _ => throw new Exception(),
                    (_, _) => throw new Exception()
                )
            )
            .AddScoped<ICommandHandler<FakeCommand>, FakeCommandHandler>
            (
                _ => new FakeCommandHandler
                (
                    fc => throw new Exception()
                )
            )
            .BuildServiceProvider();

        await provider.GetRequiredService<CommandProcessor>().ProcessCommand
            (new FakeEmptyNostaleClient(), fakeCommand);
    }

    /// <summary>
    /// Tests that exceptions are handled.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    public async Task ProcessCommand_HavingExceptionInPostEvent_ShouldNotThrow()
    {
        var fakeCommand = new FakeCommand("asdf");
        var provider = new ServiceCollection()
            .AddSingleton<CommandProcessor>()
            .AddScoped<IPreCommandExecutionEvent>
            (
                _ => new CommandEvent<FakeCommand>
                (
                    _ => Result.FromSuccess(),
                    (_, _) => throw new Exception()
                )
            )
            .AddScoped<ICommandHandler<FakeCommand>, FakeCommandHandler>
            (
                _ => new FakeCommandHandler
                (
                    fc => Result.FromSuccess()
                )
            )
            .BuildServiceProvider();

        await provider.GetRequiredService<CommandProcessor>().ProcessCommand
            (new FakeEmptyNostaleClient(), fakeCommand);
    }
}