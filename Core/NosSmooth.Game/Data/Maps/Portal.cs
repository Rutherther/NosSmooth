//
//  Portal.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Game.Data.Info;
using NosSmooth.Packets.Enums;

namespace NosSmooth.Game.Data.Maps;

/// <summary>
/// Represents map portal leading to another map.
/// </summary>
/// <param name="PortalId">The portal id.</param>
/// <param name="Position">The position of the portal.</param>
/// <param name="TargetMapId">The id of the target map.</param>
public record Portal
(
    long PortalId,
    Position Position,
    long TargetMapId,
    PortalType? PortalType,
    bool IsDisabled
);