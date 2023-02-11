//
//  ConnectionData.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Net;

namespace NosSmooth.Pcap;

public record ConnectionData
(
    IPAddress SourceAddress,
    int SourcePort,
    IPAddress DestinationAddress,
    int DestinationPort,
    List<byte[]> SniffedData,
    DateTimeOffset FirstObservedAt
);