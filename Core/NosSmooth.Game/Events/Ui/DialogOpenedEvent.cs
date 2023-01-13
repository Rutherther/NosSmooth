//
//  DialogOpenedEvent.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Game.Data.Dialogs;

namespace NosSmooth.Game.Events.Ui;

public record DialogOpenedEvent(Dialog Dialog) : IGameEvent;