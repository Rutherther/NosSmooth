//
//  PetWalkCommandHandler.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.Options;
using NosSmooth.Core.Client;
using NosSmooth.Core.Commands;
using NosSmooth.Core.Commands.Control;
using NosSmooth.Core.Commands.Walking;
using NosSmooth.LocalBinding.Objects;
using Remora.Results;

namespace NosSmooth.LocalClient.CommandHandlers.Walk;

/// <summary>
/// Handles <see cref="PetWalkCommand"/>.
/// </summary>
public class PetWalkCommandHandler : ICommandHandler<PetWalkCommand>
{
    /// <summary>
    /// Group that is used for <see cref="TakeControlCommand"/>.
    /// </summary>
    public const string PetWalkControlGroup = "PetWalk";

    private readonly PetManagerBinding _petManagerBinding;
    private readonly INostaleClient _nostaleClient;
    private readonly WalkCommandHandlerOptions _options;

    /// <summary>
    /// Initializes a new instance of the <see cref="PetWalkCommandHandler"/> class.
    /// </summary>
    /// <param name="petManagerBinding">The character object binding.</param>
    /// <param name="nostaleClient">The nostale client.</param>
    /// <param name="options">The options.</param>
    public PetWalkCommandHandler
    (
        PetManagerBinding petManagerBinding,
        INostaleClient nostaleClient,
        IOptions<WalkCommandHandlerOptions> options
    )
    {
        _options = options.Value;
        _petManagerBinding = petManagerBinding;
        _nostaleClient = nostaleClient;
    }

    /// <inheritdoc/>
    public async Task<Result> HandleCommand(PetWalkCommand command, CancellationToken ct = default)
    {
        if (_petManagerBinding.PetManagerList.Length < command.PetSelector + 1)
        {
            return new NotFoundError("Could not find the pet using the given selector.");
        }
        var petManager = _petManagerBinding.PetManagerList[command.PetSelector];

        var handler = new ControlCommandWalkHandler
        (
            _nostaleClient,
            (x, y) => _petManagerBinding.PetWalk(command.PetSelector, x, y),
            petManager,
            _options
        );

        return await handler.HandleCommand
        (
            command.TargetX,
            command.TargetY,
            command,
            PetWalkControlGroup + "_" + command.PetSelector,
            ct
        );
    }
}