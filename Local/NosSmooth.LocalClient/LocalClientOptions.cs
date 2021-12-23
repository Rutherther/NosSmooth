//
//  LocalClientOptions.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Core.Commands;

namespace NosSmooth.LocalClient;

/// <summary>
/// Options for <see cref="NostaleLocalClient"/>.
/// </summary>
public class LocalClientOptions
{
    /// <summary>
    /// Gets or sets whether the interception of packets should be allowed.
    /// </summary>
    public bool AllowIntercept { get; set; }

    /// <summary>
    /// Hook the packet sent method.
    /// </summary>
    /// <remarks>
    /// Packet handlers and interceptors won't be called for sent packets.
    /// </remarks>
    public bool HookPacketSend { get; set; } = true;

    /// <summary>
    /// Hook the packet received method.
    /// </summary>
    /// <remarks>
    /// Packet handlers and interceptors won't be called for received packets.
    /// </remarks>
    public bool HookPacketReceive { get; set; } = true;

    /// <summary>
    /// Whether to hook Character.Walk method. True by default.
    /// </summary>
    /// <remarks>
    /// If set to false, <see cref="WalkCommand.CancelOnUserMove"/> won't take any effect.
    /// </remarks>
    public bool HookCharacterWalk { get; set; } = true;
}