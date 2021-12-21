//
//  Act4Status.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosCore.Packets.Enumerations;

namespace NosSmooth.Game.Data.Act4;

/// <summary>
/// Status of a faction in act4
/// </summary>
/// <param name="Percentage">The percentage to Mukraju.</param>
/// <param name="Mode">The current mode.</param>
/// <param name="CurrentTime">The current time of the raid.</param>
/// <param name="TotalTime">The total time the raid will be for.</param>
/// <param name="Raid">The type of the raid.</param>
public record Act4FactionStatus
(
    short Percentage,
    Act4Mode Mode,
    long? CurrentTime,
    long? TotalTime,
    Act4Raid Raid
);