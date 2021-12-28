//
//  GenerateSerializerAttribute.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace NosSmooth.Packets.Attributes;

/// <summary>
/// Attribute for marking packets that should have their generator generated using Roslyn code generator.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class GenerateSerializerAttribute : Attribute
{
}