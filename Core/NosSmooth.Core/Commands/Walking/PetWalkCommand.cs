//
//  PetWalkCommand.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Core.Commands.Control;

namespace NosSmooth.Core.Commands.Walking;

/// <summary>
/// Walk the given pet to the specified position.
/// </summary>
/// <param name="PetSelector">Index of the pet to move.</param>
/// <param name="TargetX"></param>
/// <param name="TargetY"></param>
/// <param name="CanBeCancelledByAnother"></param>
/// <param name="WaitForCancellation"></param>
/// <param name="AllowUserCancel"></param>
public record PetWalkCommand
(
    int PetSelector,
    ushort TargetX,
    ushort TargetY,
    bool CanBeCancelledByAnother = true,
    bool WaitForCancellation = true,
    bool AllowUserCancel = true
) : ICommand, ITakeControlCommand
{
    /// <inheritdoc />
    public bool CancelOnMapChange => true;
}
