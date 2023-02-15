//
//  DialogConflictError.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Game.Apis;
using NosSmooth.Game.Data.Dialogs;
using Remora.Results;

namespace NosSmooth.Game.Errors;

/// <summary>
/// An error returned from <see cref="DialogHandler"/> in case the dialog was answered multiple times
/// and the answers are in conflict (was accepted, but tried to deny and vice versa)
/// </summary>
public record DialogConflictError(Dialog dialog, bool OriginalAccept, bool NewAccept) : ResultError("Dialog was already handled and the current response conflicts with the old one, cannot proceed.");