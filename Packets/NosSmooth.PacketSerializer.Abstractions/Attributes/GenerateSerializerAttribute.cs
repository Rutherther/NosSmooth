//
//  GenerateSerializerAttribute.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace NosSmooth.PacketSerializer.Abstractions.Attributes;

/// <summary>
/// Attribute for marking packets that should have their generator generated using Roslyn code generator.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class GenerateSerializerAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GenerateSerializerAttribute"/> class.
    /// </summary>
    /// <param name="allowInlineConverters">If true, the generator will try to generate parameter serializers for types like string, int, long so the type converter does not have to be called. This will increase performance, but the user will lose the ability to make custom serialization of these fields. Fields that may not be handled will still call the TypeConverter.</param>
    public GenerateSerializerAttribute(bool allowInlineConverters)
    {
    }
}