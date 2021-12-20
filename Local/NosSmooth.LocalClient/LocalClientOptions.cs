//
//  LocalClientOptions.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

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
}