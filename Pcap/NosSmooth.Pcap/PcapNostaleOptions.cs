//
//  PcapNostaleOptions.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace NosSmooth.Pcap;

/// <summary>
/// Options for <see cref="PcapNostaleManager"/> and <see cref="PcapNostaleClient"/>.
/// </summary>
public class PcapNostaleOptions
{
    /// <summary>
    /// Gets or sets the refresh interval of connections of NosTale processes in milliseconds. Default 10 milliseconds.
    /// </summary>
    public long ProcessRefreshInterval { get; set; } = 10;

    /// <summary>
    /// Gets or sets the time data from a connection should be kept in milliseconds. Default 10 seconds.
    /// </summary>
    public long CleanSniffedDataInterval { get; set; } = 10 * 1000;

    /// <summary>
    /// Gets or sets the time tcp connection should be forgotten in milliseconds. Default 10 minutes.
    /// </summary>
    public long ForgetConnectionInterval { get; set; } = 10 * 60 * 1000;
}