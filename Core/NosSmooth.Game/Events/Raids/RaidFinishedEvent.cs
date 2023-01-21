//
//  RaidFinishedEvent.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Game.Data.Raids;

namespace NosSmooth.Game.Events.Raids;

/// <summary>
/// The raid has been finished.
/// </summary>
/// <remarks>
/// There are multiple possibilities:
/// 1. The character has left.
/// 2. The raid was cancelled.
/// 3. The raid has failed. (either the whole team failed or the character)
/// 4. The raid has succeeded.
///
/// To determine which of these is the one that caused the finish
/// of the raid, look at <see cref="Raid"/> state.
/// </remarks>
/// <param name="Raid">The raid that has finished.</param>
public record RaidFinishedEvent(Raid Raid) : IGameEvent;