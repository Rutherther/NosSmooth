//
//  PacketIndexAttributeGenerator.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.CodeDom.Compiler;
using NosSmooth.PacketSerializersGenerator.Data;
using NosSmooth.PacketSerializersGenerator.Errors;
using NosSmooth.PacketSerializersGenerator.Extensions;

namespace NosSmooth.PacketSerializersGenerator.AttributeGenerators;

/// <inheritdoc />
public class PacketIndexAttributeGenerator : IParameterGenerator
{
    /// <summary>
    /// Gets the full name of the packet index attribute.
    /// </summary>
    public static string PacketIndexAttributeFullName => "NosSmooth.Packets.Attributes.PacketIndexAttribute";

    /// <inheritdoc />
    public bool ShouldHandle(ParameterInfo parameter)
        => parameter.Attributes.Any(x => x.FullName == PacketIndexAttributeFullName);

    /// <inheritdoc />
    public IError? GenerateSerializerPart(IndentedTextWriter textWriter, PacketInfo packetInfo)
    {
        bool pushedLevel = false;
        var generator = new ConverterSerializationGenerator(textWriter);
        var parameter = packetInfo.Parameters.Current;
        var attribute = parameter.Attributes.First(x => x.FullName == PacketIndexAttributeFullName);

        var afterSeparator = attribute.GetNamedValue<char?>("AfterSeparator", null);
        if (afterSeparator is not null)
        {
            generator.SetAfterSeparatorOnce((char)afterSeparator);
        }

        var innerSeparator = attribute.GetNamedValue<char?>("InnerSeparator", null);
        if (innerSeparator is not null)
        {
            generator.PushLevel((char)innerSeparator);
            pushedLevel = true;
        }

        generator.SerializeAndCheck(parameter);

        if (pushedLevel)
        {
            generator.PopLevel();
        }

        return null;
    }

    /// <inheritdoc />
    public IError? GenerateDeserializerPart(IndentedTextWriter textWriter, PacketInfo packetInfo)
    {
        bool pushedLevel = false;
        var generator = new ConverterDeserializationGenerator(textWriter);
        var parameter = packetInfo.Parameters.Current;
        var attribute = parameter.Attributes.First();

        var afterSeparator = attribute.GetNamedValue<char?>("AfterSeparator", null);
        if (afterSeparator is not null)
        {
            generator.SetAfterSeparatorOnce((char)afterSeparator);
        }

        var innerSeparator = attribute.GetNamedValue<char?>("InnerSeparator", null);
        if (innerSeparator is not null)
        {
            generator.PushLevel((char)innerSeparator);
            pushedLevel = true;
        }

        generator.DeserializeAndCheck($"{packetInfo.Namespace}.{packetInfo.Name}", parameter, packetInfo.Parameters.IsLast);

        if (!parameter.Nullable)
        {
            generator.CheckNullError(parameter.GetResultVariableName(), parameter.Name);
        }

        generator.AssignLocalVariable(parameter);

        if (pushedLevel)
        {
            generator.ReadToLastToken();
            generator.PopLevel();
        }

        return null;
    }
}