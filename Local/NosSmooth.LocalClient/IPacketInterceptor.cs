//
//  IPacketInterceptor.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace NosSmooth.LocalClient;

/// <summary>
/// Used for intercepting packet communication,
/// changing the contents of the packets and/or cancelling them altogether.
/// </summary>
public interface IPacketInterceptor
{
    /// <summary>
    /// Intercept the given packet.
    /// </summary>
    /// <param name="packet">The packet itself, if it is changed, the modified packet will be sent.</param>
    /// <returns>Whether to send the packet to the server.</returns>
    public bool InterceptSend(ref string packet);

    /// <summary>
    /// Intercept the given packet.
    /// </summary>
    /// <param name="packet">The packet itself, if it is changed, the modified packet will be received.</param>
    /// <returns>Whether to receive the packet by the client.</returns>
    public bool InterceptReceive(ref string packet);
}