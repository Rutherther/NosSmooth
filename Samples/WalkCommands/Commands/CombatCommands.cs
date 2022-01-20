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
    private readonly FeedbackService _feedbackService;

    /// <summary>
    /// Initializes a new instance of the <see cref="CombatCommands"/> class.
    /// </summary>
    /// <param name="sceneManagerBinding">The scene manager binding.</param>
    /// <param name="feedbackService">The feedback service.</param>
    public CombatCommands(SceneManagerBinding sceneManagerBinding, FeedbackService feedbackService)
    {
        _sceneManagerBinding = sceneManagerBinding;
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
}