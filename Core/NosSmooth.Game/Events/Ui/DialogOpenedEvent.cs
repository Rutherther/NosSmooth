//
//  DialogOpenedEvent.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Game.Apis;
using NosSmooth.Game.Data.Dialogs;

namespace NosSmooth.Game.Events.Ui;

/// <summary>
/// A dialog has been opened. An answer is expected.
/// </summary>
/// <remarks>
/// For answering to a dialog, use <see cref="DialogHandler"/>.
/// It takes care of collisions. In case the same dialog is accepted
/// and denied from elsewhere, an error will be returned.
/// </remarks>
/// <param name="Dialog">The dialog that has been opened.</param>
public record DialogOpenedEvent(Dialog Dialog) : IGameEvent;