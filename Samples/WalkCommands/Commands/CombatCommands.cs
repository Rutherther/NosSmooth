//
//  CombatCommands.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.ChatCommands;
using NosSmooth.Core.Client;
using NosSmooth.LocalBinding;
using NosSmooth.LocalBinding.Objects;
using NosSmooth.LocalBinding.Structs;
using Remora.Commands.Attributes;
using Remora.Commands.Groups;
using Remora.Results;

namespace WalkCommands.Commands;

/// <summary>
/// Represents command group for combat commands.
/// </summary>
public class CombatCommands : CommandGroup
{
    private readonly UnitManagerBinding _unitManagerBinding;
    private readonly SceneManager _sceneManager;
    private readonly PlayerManagerBinding _playerManagerBinding;
    private readonly FeedbackService _feedbackService;

    /// <summary>
    /// Initializes a new instance of the <see cref="CombatCommands"/> class.
    /// </summary>
    /// <param name="unitManagerBinding">The scene manager binding.</param>
    /// <param name="sceneManager">The scene manager.</param>
    /// <param name="playerManagerBinding">The character binding.</param>
    /// <param name="feedbackService">The feedback service.</param>
    public CombatCommands
    (
        UnitManagerBinding unitManagerBinding,
        SceneManager sceneManager,
        PlayerManagerBinding playerManagerBinding,
        FeedbackService feedbackService
    )
    {
        _unitManagerBinding = unitManagerBinding;
        _sceneManager = sceneManager;
        _playerManagerBinding = playerManagerBinding;
        _feedbackService = feedbackService;
    }

    /// <summary>
    /// Focus the given entity.
    /// </summary>
    /// <param name="entityId">The entity id to focus.</param>
    /// <returns>A task that may or may not have succeeded.</returns>
    [Command("focus")]
    public Task<Result> HandleFocusAsync(int entityId)
    {
        var entityResult = _sceneManager.FindEntity(entityId);
        if (!entityResult.IsSuccess)
        {
            return Task.FromResult(Result.FromError(entityResult));
        }

        return Task.FromResult(_unitManagerBinding.FocusEntity(entityResult.Entity));
    }

    /// <summary>
    /// Follow the given entity.
    /// </summary>
    /// <param name="entityId">The entity id to follow.</param>
    /// <returns>A task that may or may not have succeeded.</returns>
    [Command("follow")]
    public Task<Result> HandleFollowAsync(int entityId)
    {
        var entityResult = _sceneManager.FindEntity(entityId);
        if (!entityResult.IsSuccess)
        {
            return Task.FromResult(Result.FromError(entityResult));
        }

        return Task.FromResult(_playerManagerBinding.FollowEntity(entityResult.Entity));
    }

    /// <summary>
    /// Stop following an entity.
    /// </summary>
    /// <returns>A task that may or may not have succeeded.</returns>
    [Command("unfollow")]
    public Task<Result> HandleUnfollowAsync()
    {
        return Task.FromResult(_playerManagerBinding.UnfollowEntity());
    }
}