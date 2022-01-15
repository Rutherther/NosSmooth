//
//  NetworkBindingOptions.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.LocalBinding.Objects;

namespace NosSmooth.LocalBinding.Options;

/// <summary>
/// Options for <see cref="NetworkBinding"/>.
/// </summary>
public class NetworkBindingOptions
{
    /// <summary>
    /// Gets or sets whether to hook the send packet function.
    /// </summary>
    public bool HookSend { get; set; } = true;

    /// <summary>
    /// Gets or sets whether to hook the receive packet function.
    /// </summary>
    public bool HookReceive { get; set; } = true;

    /// <summary>
    /// Gets or sets the pattern to find the network object at.
    /// </summary>
    /// <remarks>
    /// The address of the object is "three pointers down" from address found on this pattern.
    /// </remarks>
    public string NetworkObjectPattern { get; set; }
        = "A1 ?? ?? ?? ?? 8B 00 BA ?? ?? ?? ?? E8 ?? ?? ?? ?? E9 ?? ?? ?? ?? A1 ?? ?? ?? ?? 8B 00 8B 40 40";

    /// <summary>
    /// Gets or sets the pattern to find the send packet function at.
    /// </summary>
    public string SendFunctionPattern { get; set; } = "53 56 8B F2 8B D8 EB 04";

    /// <summary>
    /// Gets or sets the pattern to find the receive function at.
    /// </summary>
    public string ReceiveFunctionPattern { get; set; } = "55 8B EC 83 C4 F4 53 56 57 33 C9 89 4D F4 89 55 FC 8B D8 8B 45 FC";
}