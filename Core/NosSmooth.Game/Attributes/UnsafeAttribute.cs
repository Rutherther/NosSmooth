//
//  UnsafeAttribute.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace NosSmooth.Game.Attributes;

/// <summary>
/// The given method does not do some checks.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Struct)]
public class UnsafeAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UnsafeAttribute"/> class.
    /// </summary>
    /// <param name="reason">The reason.</param>
    public UnsafeAttribute(string reason)
    {
        Reason = reason;
    }

    /// <summary>
    /// Gets the unsafe reason.
    /// </summary>
    public string Reason { get; }
}