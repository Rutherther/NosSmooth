//
//  WalkCommandHandlerTests.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using NosSmooth.Core.Commands.Control;
using NosSmooth.Core.Commands.Walking;
using NosSmooth.Core.Tests.Fakes;
using Remora.Results;
using Xunit;

namespace NosSmooth.Core.Tests.Commands.Walking;

/// <summary>
/// Tests handling walk command.
/// </summary>
public class WalkCommandHandlerTests
{
    /// <summary>
    /// Tests that pet and player walk commands will be called.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    public async Task Handle_CallsPetAndPlayerWalkCommands()
    {
        var calledPetWalk = false;
        var calledPlayerWalk = false;
        var command = new WalkCommand(0, 0, new[] { 1, 2 }, 0);
        var walkHandler = new WalkCommandHandler
        (
            new FakeNostaleClient
            (
                (c, _) =>
                {
                    if (c is PlayerWalkCommand)
                    {
                        calledPlayerWalk = true;
                    }
                    if (c is PetWalkCommand)
                    {
                        calledPetWalk = true;
                    }
                    return Result.FromSuccess();
                }
            )
        );

        await walkHandler.HandleCommand(command);
        Assert.True(calledPetWalk);
        Assert.True(calledPlayerWalk);
    }

    /// <summary>
    /// Tests that handling will preserve the <see cref="ITakeControlCommand"/> properties.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    public async Task Handle_PreservesTakeHandlerCommandProperties()
    {
        var command = new WalkCommand
        (
            0,
            0,
            new[] { 2, 5, 7, 9 },
            0,
            true,
            false,
            false
        );
        var walkHandler = new WalkCommandHandler
        (
            new FakeNostaleClient
            (
                (c, _) =>
                {
                    if (c is ITakeControlCommand takeControl)
                    {
                        Assert.Equal(command.AllowUserCancel, takeControl.AllowUserCancel);
                        Assert.Equal(command.WaitForCancellation, takeControl.WaitForCancellation);
                        Assert.Equal(command.CancelOnMapChange, takeControl.CancelOnMapChange);
                        Assert.Equal(command.CanBeCancelledByAnother, takeControl.CanBeCancelledByAnother);
                    }
                    return Result.FromSuccess();
                }
            )
        );

        await walkHandler.HandleCommand(command);
    }

    /// <summary>
    /// Tests that handler preserves the position to player walk command.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    public async Task Handle_PreservesPlayerWalkPosition()
    {
        var command = new WalkCommand
        (
            10,
            15,
            Array.Empty<int>(),
            0,
            true,
            false,
            false
        );
        var walkHandler = new WalkCommandHandler
        (
            new FakeNostaleClient
            (
                (c, _) =>
                {
                    if (c is PlayerWalkCommand playerWalkCommand)
                    {
                        Assert.Equal(command.TargetX, playerWalkCommand.TargetX);
                        Assert.Equal(command.TargetY, playerWalkCommand.TargetY);
                        Assert.Equal(command.ReturnDistanceTolerance, playerWalkCommand.ReturnDistanceTolerance);
                    }
                    return Result.FromSuccess();
                }
            )
        );

        await walkHandler.HandleCommand(command);
    }

    /// <summary>
    /// Tests that the handler will be called for every pet.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    public async Task Handle_WithPets_IsCalledForEveryPet()
    {
        var calledCount = 0;
        var command = new WalkCommand
        (
            10,
            15,
            new[] { 1, 2, 5, 7, 8 },
            0,
            true,
            false,
            false
        );
        var walkHandler = new WalkCommandHandler
        (
            new FakeNostaleClient
            (
                (c, _) =>
                {
                    if (c is PetWalkCommand petWalkCommand)
                    {
                        if (command.PetSelectors.Contains(petWalkCommand.PetSelector))
                        {
                            calledCount++;
                        }
                        else
                        {
                            throw new ArgumentException("Pet command was called for non-selected pet.");
                        }
                    }
                    return Result.FromSuccess();
                }
            )
        );

        await walkHandler.HandleCommand(command);
        Assert.Equal(command.PetSelectors.Length, calledCount);
    }

    /// <summary>
    /// Tests that pet commands will have correct position set.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    public async Task Handle_WithPets_UsesNearbyPositionForPetCommands()
    {
        var command = new WalkCommand
        (
            10,
            15,
            new[] { 1, 2, 5, 7, 8 },
            0,
            true,
            false,
            false
        );
        var walkHandler = new WalkCommandHandler
        (
            new FakeNostaleClient
            (
                (c, _) =>
                {
                    if (c is PetWalkCommand petWalkCommand)
                    {
                        Assert.True((command.TargetX - petWalkCommand.TargetX) <= 3);
                        Assert.True((command.TargetY - petWalkCommand.TargetY) <= 3);
                        Assert.Equal(command.ReturnDistanceTolerance, petWalkCommand.ReturnDistanceTolerance);
                    }
                    return Result.FromSuccess();
                }
            )
        );

        await walkHandler.HandleCommand(command);
    }
}