//
//  Dialog.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Game.Apis;
using NosSmooth.Packets.Enums;
using OneOf;

namespace NosSmooth.Game.Data.Dialogs;

/// <summary>
/// Represents dialog sent by the server
/// </summary>
/// <remarks>
/// To deny a dialog with null <see cref="DenyCommand"/>, just ignore it.
///
/// For answering to a dialog, use <see cref="DialogHandler"/>.
/// It takes care of collisions. In case the same dialog is accepted
/// and denied from elsewhere, an error will be returned.
/// </remarks>
/// <param name="AcceptCommand">The accept command/packet sent upon accept.</param>
/// <param name="DenyCommand">The deny command/packet sent upon denying. This may be null for some dialogs. To deny a dialog with null DenyCommand, just ignore it.</param>
/// <param name="Message">The message of the dialog, may be a constant i18n message, or a string.</param>
/// <param name="Parameters">The parameters of the i18n message. Empty for string messages.</param>
public record Dialog
(
    string AcceptCommand,
    string? DenyCommand,
    OneOf<string, Game18NConstString> Message,
    IReadOnlyList<string> Parameters
);