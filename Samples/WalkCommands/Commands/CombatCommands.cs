//
//  CombatCommands.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.ChatCommands;
using NosSmooth.Core.Client;
using NosSmooth.LocalBinding.Objects;
using Remora.Commands.Attributes;
using Remora.Commands.Groups;
using Remora.Results;

namespace WalkCommands.Commands;

/// <summary>
/// Represents command group for combat commands.
/// </summary>
public class CombatCommands : CommandGroup
{
    private readonly SceneManagerBinding _sceneManagerBinding;
    private readonly CharacterBinding _characterBinding;
    private readonly FeedbackService _feedbackService;

    /// <summary>
    /// Initializes a new instance of the <see cref="CombatCommands"/> class.
    /// </summary>
    /// <param name="sceneManagerBinding">The scene manager binding.</param>
    /// <param name="characterBinding">The character binding.</param>
    /// <param name="feedbackService">The feedback service.</param>
    public CombatCommands(SceneManagerBinding sceneManagerBinding, CharacterBinding characterBinding, FeedbackService feedbackService)
    {
        _sceneManagerBinding = sceneManagerBinding;
        _characterBinding = characterBinding;
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
        return Task.FromResult(_sceneManagerBinding.FocusEntity(entityId));
    }

    /// <summary>
    /// Follow the given entity.
    /// </summary>
    /// <param name="entityId">The entity id to follow.</param>
    /// <returns>A task that may or may not have succeeded.</returns>
    [Command("follow")]
    public Task<Result> HandleFollowAsync(int entityId)
    {
        var entity = _sceneManagerBinding.FindEntity(entityId);
        if (!entity.IsSuccess)
        {
            return Task.FromResult(Result.FromError(entity));
        }

        return Task.FromResult(_characterBinding.FollowEntity(entity.Entity));
    }

    /// <summary>
    /// Stop following an entity.
    /// </summary>
    /// <returns>A task that may or may not have succeeded.</returns>
    [Command("unfollow")]
    public Task<Result> HandleUnfollowAsync()
    {
        return Task.FromResult(_characterBinding.UnfollowEntity());
    }
}