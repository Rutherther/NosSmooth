//
//  CanBeUsedResponse.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace NosSmooth.Extensions.Combat.Operations;

/// <summary>
/// A response to <see cref="ICombatOperation"/> CanBeUsed method.
/// </summary>
public enum CanBeUsedResponse
{
    /// <summary>
    /// The operation may be used right awayt.
    /// </summary>
    CanBeUsed,

    /// <summary>
    /// The operation will be usable after some amount of time.
    /// </summary>
    MustWait,

    /// <summary>
    /// The operation won't be usable. (ie. missing arrows).
    /// </summary>
    WontBeUsable
}