//
//  Message.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace NosSmooth.LocalBinding.Interop;

/// <summary>
/// A Windows message.
/// </summary>
public struct Message
{
    /// <summary>
    /// Gets or sets the message code.
    /// </summary>
    public WindowsMessage Msg { get; set; }

    /// <summary>
    /// Gets or sets the first parameter.
    /// </summary>
    public IntPtr FirstParam { get; set; }

    /// <summary>
    /// Gets or sets the second parameter.
    /// </summary>
    public IntPtr SecondParam { get; set; }

    /// <summary>
    /// Gets or sets the pointer to the result.
    /// </summary>
    public IntPtr Result { get; set; }
}