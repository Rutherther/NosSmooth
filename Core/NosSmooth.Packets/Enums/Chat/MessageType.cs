//
//  Message.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace NosSmooth.Packets.Enums.Chat;

/// <summary>
/// The type of the message.
/// </summary>
public enum MessageType
{
    /// <summary>
    /// Default message in chat.
    /// </summary>
    Default = 0,

    /// <summary>
    /// Unknown.
    /// </summary>
    Notification = 1,

    /// <summary>
    /// The message is a shout (shown at the top of screen on the whole width.)
    /// </summary>
    Shout = 2,

    /// <summary>
    /// The message is shown at the center of the screen.
    /// </summary>
    Center = 3,

    /// <summary>
    /// The message is shown at the bottom center of the screen.
    /// </summary>
    /// <remarks>
    /// Only few players with the highest reputation can send this..
    /// </remarks>
    Hero = 4,

    /// <summary>
    /// Unknown.
    /// </summary>
    DefaultAndNotification
}