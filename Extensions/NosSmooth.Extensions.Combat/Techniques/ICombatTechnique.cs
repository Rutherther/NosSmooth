//
//  ICombatTechnique.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Extensions.Combat.Operations;
using OneOf.Types;
using Remora.Results;

namespace NosSmooth.Extensions.Combat.Techniques;

/// <summary>
/// A combat technique that allows to handle the whole combat situations using step callbacks.
/// </summary>
/// <remarks>
/// The callback methods decide the next steps, used in <see cref="CombatManager"/>.
/// </remarks>
public interface ICombatTechnique
{
    public bool ShouldContinue(ICombatState state);

    public Result HandleCombatStep(ICombatState state);

    public Result HandleError(ICombatState state, ICombatOperation operation, Result result);
}