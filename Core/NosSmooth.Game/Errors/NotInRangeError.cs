//
//  NotInRangeError.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Game.Data.Info;
using Remora.Results;

namespace NosSmooth.Game.Errors;

public record NotInRangeError
(
    string Entity,
    Position EntityPosition,
    Position TargetPosition,
    short Range
) : ResultError($"Entity {Entity} ({EntityPosition}) is not in range ({Range}) of ({TargetPosition}).");