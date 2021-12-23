//
//  WalkCommandHandler.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Core.Commands;
using NosSmoothCore;
using Remora.Results;

namespace NosSmooth.LocalClient.CommandHandlers;

/// <summary>
/// Handles <see cref="WalkCommand"/>.
/// </summary>
public class WalkCommandHandler : ICommandHandler<WalkCommand>
{
    private readonly NosClient _nosClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="WalkCommandHandler"/> class.
    /// </summary>
    /// <param name="nosClient">The local client.</param>
    public WalkCommandHandler(NosClient nosClient)
    {
        _nosClient = nosClient;
    }

    /// <inheritdoc/>
    public Task<Result> HandleCommand(WalkCommand command, CancellationToken ct = default)
    {
        _nosClient.GetCharacter().Walk(command.TargetX, command.TargetY);
        return Task.Delay(1000).ContinueWith(_ => Result.FromSuccess()); // TODO: Wait for the move to finish
    }
}