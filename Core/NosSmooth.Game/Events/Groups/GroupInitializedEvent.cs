//
//  GroupInitializedEvent.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Game.Data.Social;

namespace NosSmooth.Game.Events.Groups;

/// <summary>
/// A group has been initialized.
/// </summary>
/// <remarks>
/// May be sent multiple times even for the same group.
/// </remarks>
/// <param name="Group">The initialized group with members.</param>
public record GroupInitializedEvent(Group Group) : IGameEvent;