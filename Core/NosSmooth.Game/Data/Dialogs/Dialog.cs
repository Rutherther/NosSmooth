//
//  Dialog.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace NosSmooth.Game.Data.Dialogs;

/// <summary>
/// Represents dialog sent by the server
/// </summary>
/// <param name="AcceptCommand">The accept command sent upon accept.</param>
/// <param name="Parameters">The parameters of the dialog.</param>
public record Dialog
(
    string AcceptCommand,

    // OneOf<Game18NConstString, string> Message,
    IReadOnlyList<string> Parameters
);