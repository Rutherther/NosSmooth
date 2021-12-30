//
//  PacketContextListAttributeGenerator.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.CodeDom.Compiler;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NosSmooth.PacketSerializersGenerator.Data;
using NosSmooth.PacketSerializersGenerator.Errors;
using NosSmooth.PacketSerializersGenerator.Extensions;

namespace NosSmooth.PacketSerializersGenerator.AttributeGenerators;

/// <inheritdoc />
public class PacketContextListAttributeGenerator : IParameterGenerator
{
    /// <summary>
    /// Gets the full name of the packet index attribute.
    /// </summary>
    public static string PacketListIndexAttributeFullName => "NosSmooth.Packets.Attributes.PacketContextListAttribute";

    /// <inheritdoc />
    public bool ShouldHandle(ParameterInfo parameter)
        => parameter.Attributes.Any(x => x.FullName == PacketListIndexAttributeFullName);

    /// <inheritdoc />
    public IError? GenerateSerializerPart(IndentedTextWriter textWriter, PacketInfo packetInfo)
    {
        var generator = new ConverterSerializationGenerator(textWriter);
        var parameter = packetInfo.Parameters.Current;
        var attribute = parameter.Attributes.First(x => x.FullName == PacketListIndexAttributeFullName);

        var afterSeparator = attribute.GetNamedValue<char?>("AfterSeparator", null);
        if (afterSeparator is not null)
        {
            generator.SetAfterSeparatorOnce((char)afterSeparator);
        }

        var listSeparator = attribute.GetNamedValue<char>("ListSeparator", '|');
        generator.PushLevel(listSeparator);

        var innerSeparator = attribute.GetNamedValue<char>("InnerSeparator", '.');
        generator.PrepareLevel(innerSeparator);

        generator.SerializeAndCheck(parameter);
        generator.RemovePreparedLevel();
        generator.PopLevel();

        return null;
    }

    /// <inheritdoc />
    public IError? GenerateDeserializerPart(IndentedTextWriter textWriter, PacketInfo packetInfo)
    {
        var generator = new ConverterDeserializationGenerator(textWriter);
        var parameter = packetInfo.Parameters.Current;
        var attribute = parameter.Attributes.First(x => x.FullName == PacketListIndexAttributeFullName);

        var afterSeparator = attribute.GetNamedValue<char?>("AfterSeparator", null);
        if (afterSeparator is not null)
        {
            generator.SetAfterSeparatorOnce((char)afterSeparator);
        }

        var listSeparator = attribute.GetNamedValue<char>("ListSeparator", '|');
        var lengthVariable = attribute.GetIndexedValue<string>(1);
        textWriter.WriteLine(@$"stringEnumerator.PushLevel(""{listSeparator}"", {lengthVariable});");

        var innerSeparator = attribute.GetNamedValue<char>("InnerSeparator", '.');
        generator.PrepareLevel(innerSeparator);

        generator.DeserializeAndCheck($"{packetInfo.Namespace}.{packetInfo.Name}", parameter, packetInfo.Parameters.IsLast);
        generator.RemovePreparedLevel();
        generator.PopLevel();
        if (!parameter.Nullable)
        {
            generator.CheckNullError(parameter.GetResultVariableName(), parameter.Name);
        }

        generator.AssignLocalVariable(parameter);

        return null;
    }
}