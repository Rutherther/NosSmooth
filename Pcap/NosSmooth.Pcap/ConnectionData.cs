//
//  ConnectionData.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Net;

namespace NosSmooth.Pcap;

/// <summary>
/// Data from a tcp connection containing first few sniffed packets.
/// </summary>
/// <param name="SourceAddress">The packets source address.</param>
/// <param name="SourcePort">The packets source port.</param>
/// <param name="DestinationAddress">The packets destination address.</param>
/// <param name="DestinationPort">The packets destination port.</param>
/// <param name="SniffedData">The sniffed data.</param>
/// <param name="FirstObservedAt">The time first data were observed at.</param>
public record ConnectionData
(
    IPAddress SourceAddress,
    int SourcePort,
    IPAddress DestinationAddress,
    int DestinationPort,
    List<byte[]> SniffedData,
    DateTimeOffset FirstObservedAt
);