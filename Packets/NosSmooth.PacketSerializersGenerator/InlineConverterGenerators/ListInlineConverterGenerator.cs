//
//  ListInlineConverterGenerator.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.CodeDom.Compiler;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NosSmooth.PacketSerializersGenerator.Errors;
using NosSmooth.PacketSerializersGenerator.Extensions;

namespace NosSmooth.PacketSerializersGenerator.InlineConverterGenerators;

/// <inheritdoc/>
public class ListInlineConverterGenerator : IInlineConverterGenerator
{
    private readonly InlineTypeConverterGenerator _inlineConverters;
    private readonly List<ITypeSymbol> _listTypes;

    /// <summary>
    /// Initializes a new instance of the <see cref="ListInlineConverterGenerator"/> class.
    /// </summary>
    /// <param name="inlineConverters">The inline converter that is used for converting the values of the list.</param>
    public ListInlineConverterGenerator(InlineTypeConverterGenerator inlineConverters)
    {
        _inlineConverters = inlineConverters;
        _listTypes = new List<ITypeSymbol>();
    }

    /// <inheritdoc />
    public bool ShouldHandle(TypeSyntax? typeSyntax, ITypeSymbol? typeSymbol)
        => typeSymbol?.Name == "IReadOnlyList";

    /// <inheritdoc />
    public IError? GenerateSerializerPart
    (
        IndentedTextWriter textWriter,
        string variableName,
        TypeSyntax? typeSyntax,
        ITypeSymbol? typeSymbol
    )
    {
        ITypeSymbol genericArgument = ((INamedTypeSymbol)typeSymbol!).TypeArguments[0];
        textWriter.WriteMultiline
        (
            @$"
        if ({variableName} is null)
        {{
            builder.Append('-');
        }}
        else
        {{
            foreach (var item in {variableName})
            {{
                if (!builder.PushPreparedLevel())
                {{
                    return new ArgumentInvalidError(nameof(builder), ""The string builder has to have a prepared level for all lists."");
                }}

"
        );
        var error = _inlineConverters.Serialize(textWriter, "item", null, genericArgument);
        if (error is not null)
        {
            return error;
        }

        textWriter.WriteMultiline(@"
                builder.PopLevel();
            }
        }
"
        );
        return null;
    }

    /// <inheritdoc />
    public IError? CallDeserialize(IndentedTextWriter textWriter, TypeSyntax? typeSyntax, ITypeSymbol? typeSymbol, bool nullable)
    {
        ITypeSymbol genericArgument = ((INamedTypeSymbol)typeSymbol!).TypeArguments[0];
        if (_listTypes.All
            (
                x => x.ToString() != genericArgument!.ToString()
                    || ((x.IsNullable() ?? false) && (!genericArgument.IsNullable() ?? false))
            ))
        {
            _listTypes.Add(genericArgument!);
        }

        textWriter.WriteLine
            ($"{Constants.HelperClass}.{GetMethodName(genericArgument, nullable)}(typeConverter, _stringSerializer, in stringEnumerator);");
        return null;
    }

    private string GetMethodName(ITypeSymbol genericArgumentType, bool nullable)
    {
        return
            $"ParseList{genericArgumentType.ToString().TrimEnd('?').Replace('.', '_').Replace('<', '_').Replace('>', '_')}{((genericArgumentType.IsNullable() ?? false) ? "Nullable" : string.Empty)}{(nullable ? "NullableSupport" : string.Empty)}";
    }

    private void GenerateHelperMethods(IndentedTextWriter textWriter, bool nullable)
    {
        foreach (var type in _listTypes)
        {
            textWriter.WriteLine
            (
                @$"
public static Result<IReadOnlyList<{type.GetActualType()}>> {GetMethodName(type, nullable)}(IStringConverter typeConverter, IStringSerializer _stringSerializer, in PacketStringEnumerator stringEnumerator)
{{
    var data = new List<{type.GetActualType()}>();

    while (!(stringEnumerator.IsOnLastToken() ?? false))
    {{
        stringEnumerator.CaptureReadTokens();
        if (!stringEnumerator.PushPreparedLevel())
        {{
            return new ArgumentInvalidError(nameof(stringEnumerator), ""The string enumerator has to have a prepared level for all lists."");
        }}

        var result = "
            );
            /*var error = */_inlineConverters.CallDeserialize(textWriter, null, type, nullable); // TODO handle error

            textWriter.WriteMultiline(@$"

        // If we know that we are not on the last token in the item level, just skip to the end of the item.
        // Note that if this is the case, then that means the converter is either corrupted
        // or the packet has more fields.
        while (stringEnumerator.IsOnLastToken() == false)
        {{
            stringEnumerator.GetNextToken(out _);
        }}

        stringEnumerator.PopLevel();
        stringEnumerator.IncrementReadTokens();
        if (!result.IsSuccess)
        {{
            return Result<IReadOnlyList<{type}>>.FromError(new ListSerializerError(result, data.Count), result);
        }}

"
            );

            if (!(type.IsNullable() ?? false))
            {
                textWriter.WriteMultiline
                (
                    $@"
if (result.Entity is null)
{{
    return new DeserializedValueNullError(typeof({type.ToString().TrimEnd('?')}));
}}
"
                );
            }
            textWriter.WriteLine($"data.Add(({type.GetActualType()})result.Entity);");
            textWriter.WriteLine("}");
            textWriter.WriteLine("return data;");
            textWriter.WriteLine("}");
        }
    }

    /// <inheritdoc />
    public void GenerateHelperMethods(IndentedTextWriter textWriter)
    {
        GenerateHelperMethods(textWriter, true);
        GenerateHelperMethods(textWriter, false);
    }

}