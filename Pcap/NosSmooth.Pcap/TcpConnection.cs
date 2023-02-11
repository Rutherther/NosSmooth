//
//  TcpConnection.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace NosSmooth.Pcap;

/// <summary>
/// A tcp connection.
/// </summary>
/// <param name="LocalAddr">The local address.</param>
/// <param name="LocalPort">The local port.</param>
/// <param name="RemoteAddr">The remote address.</param>
/// <param name="RemotePort">The remote port.</param>
[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification = "Fix this.")]
public record struct TcpConnection
(
    long LocalAddr,
    int LocalPort,
    long RemoteAddr,
    int RemotePort
);