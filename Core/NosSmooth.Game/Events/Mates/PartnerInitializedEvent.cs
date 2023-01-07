//
//  PartnerInitializedEvent.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Game.Data.Mates;
using NosSmooth.Game.Data.Social;

namespace NosSmooth.Game.Events.Mates;

/// <summary>
/// A partner the character owns was initialized.
/// </summary>
/// <param name="Partner">The partner.</param>
public record PartnerInitializedEvent(Partner Partner) : IGameEvent;