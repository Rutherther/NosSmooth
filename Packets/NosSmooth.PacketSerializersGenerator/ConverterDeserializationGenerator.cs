//
//  ConverterDeserializationGenerator.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.CodeDom.Compiler;
using NosSmooth.PacketSerializersGenerator.Data;
using NosSmooth.PacketSerializersGenerator.Errors;
using NosSmooth.PacketSerializersGenerator.Extensions;

namespace NosSmooth.PacketSerializersGenerator;

/// <summary>
/// Various templates for converter deserialization.
/// </summary>
public class ConverterDeserializationGenerator
{
    private readonly string _stringEnumeratorVariable = "stringEnumerator";
    private readonly IndentedTextWriter _textWriter;

    /// <summary>
    /// Initializes a new instance of the <see cref="ConverterDeserializationGenerator"/> class.
    /// </summary>
    /// <param name="textWriter">The text writer.</param>
    public ConverterDeserializationGenerator(IndentedTextWriter textWriter)
    {
        _textWriter = textWriter;
    }

    /// <summary>
    /// Push level to the string enumerator.
    /// </summary>
    /// <param name="separator">The separator.</param>
    public void SetAfterSeparatorOnce(char separator)
    {
        _textWriter.WriteLine(@$"{_stringEnumeratorVariable}.SetAfterSeparatorOnce('{separator}');");
    }

    /// <summary>
    /// Sets that the next token should be read to the last entry in the level.
    /// </summary>
    public void SetReadToLast()
    {
        _textWriter.WriteLine(@$"{_stringEnumeratorVariable}.SetReadToLast();");
    }

    /// <summary>
    /// Push level to the string enumerator.
    /// </summary>
    /// <param name="separator">The separator.</param>
    /// <param name="maxTokens">The maximum number of tokens to read.</param>
    public void PushLevel(char separator, uint? maxTokens = default)
    {
        _textWriter.WriteLine(@$"{_stringEnumeratorVariable}.PushLevel('{separator}', {maxTokens?.ToString() ?? "null"});");
    }

    /// <summary>
    /// Pop level from the string enumerator.
    /// </summary>
    public void PopLevel()
    {
        _textWriter.WriteLine($"{_stringEnumeratorVariable}.PopLevel();");
    }

    /// <summary>
    /// Prepare the level to the string enumerator.
    /// </summary>
    /// <param name="separator">The separator.</param>
    /// <param name="maxTokens">The maximum number of tokens to read.</param>
    public void PrepareLevel(char separator, uint? maxTokens = default)
    {
        _textWriter.WriteLine($@"{_stringEnumeratorVariable}.PrepareLevel('{separator}', {maxTokens?.ToString() ?? "null"});");
    }

    /// <summary>
    /// Prepare the level to the string enumerator.
    /// </summary>
    public void RemovePreparedLevel()
    {
        _textWriter.WriteLine($@"{_stringEnumeratorVariable}.RemovePreparedLevel();");
    }

    /// <summary>
    /// Try to read to the last token of the level.
    /// </summary>
    /// <remarks>
    /// If we know that we are not on the last token in the item level, just skip to the end of the item.
    /// Note that if this is the case, then that means the converter is either corrupted
    /// or the packet has more fields.
    /// </remarks>
    public void ReadToLastToken()
    {
        _textWriter.WriteLine($@"while ({_stringEnumeratorVariable}.IsOnLastToken() == false)");
        _textWriter.WriteLine("{");
        _textWriter.Indent++;
        _textWriter.WriteLine($"{_stringEnumeratorVariable}.GetNextToken(out _);");
        _textWriter.Indent--;
        _textWriter.WriteLine("}");
    }

    /// <summary>
    /// Check taht the given variable is not null, if it is, return an error.
    /// </summary>
    /// <param name="nullableVariableName">The variable to check for null.</param>
    /// <param name="resultVariableName">The result variable to use for the error.</param>
    /// <param name="parameterName">The parameter that is being parsed.</param>
    /// <param name="reason">The reason for the error.</param>
    public void CheckNullError(string nullableVariableName, string resultVariableName, string parameterName, string reason = "The converter has returned null even though it was not expected.")
    {
        _textWriter.WriteMultiline($@"
if ({nullableVariableName} is null) {{
    return new PacketParameterSerializerError(this, ""{parameterName}"", {resultVariableName}, ""{reason}"");
}}
");
    }

    /// <summary>
    /// Assign local variable with the result of the parameter deserialization.
    /// </summary>
    /// <param name="parameter">The parameter.</param>
    public void DeclareLocalVariable(ParameterInfo parameter)
    {
        _textWriter.WriteLine($"{parameter.GetActualType()} {parameter.GetVariableName()};");
    }

    /// <summary>
    /// Assign local variable with the result of the parameter deserialization.
    /// </summary>
    /// <param name="parameter">The parameter.</param>
    /// <param name="declare">Whether to also declare the local variable.</param>
    public void AssignLocalVariable(ParameterInfo parameter, bool declare = true)
    {
        _textWriter.WriteLine($"{(declare ? "var " : string.Empty)}{parameter.Name} = ({parameter.GetActualType()}){parameter.GetNullableVariableName()};");
    }

    /// <summary>
    /// Begins the if for optionals, check if the parameter is not nullable.
    /// </summary>
    /// <param name="parameter">The parameter information.</param>
    /// <param name="packetName">The name of the packet.</param>
    public void StartOptionalCheck(ParameterInfo parameter, string packetName)
    {
        // serialize this parameter only if we are not on the last token.
        _textWriter.WriteLine($"if (!(stringEnumerator.IsOnLastToken() ?? true))");
        _textWriter.WriteLine("{");
        _textWriter.Indent++;
    }

    /// <summary>
    /// Ends the if for optionals.
    /// </summary>
    /// <param name="parameter">The parameter information.</param>
    public void EndOptionalCheck(ParameterInfo parameter)
    {
        _textWriter.Indent--;
        _textWriter.WriteLine("}");
        _textWriter.WriteLine("else");
        _textWriter.WriteLine("{");
        _textWriter.Indent++;

        _textWriter.WriteLine($"{parameter.GetVariableName()} = null;");

        _textWriter.Indent--;
        _textWriter.WriteLine("}");
    }

    /// <summary>
    /// Validates that the string enumerator is currently not at the last token.
    /// </summary>
    /// <param name="parameterName">The parameter that is being converted.</param>
    public void ValidateNotLast(string parameterName)
    {
        _textWriter.WriteLine($"if ({_stringEnumeratorVariable}.IsOnLastToken() ?? false)");
        _textWriter.WriteLine("{");
        _textWriter.Indent++;
        _textWriter.WriteLine($"return new PacketEndNotExpectedError(this, \"{parameterName}\");");
        _textWriter.Indent--;
        _textWriter.WriteLine("}");
    }

    /// <summary>
    /// Call deserializer and check the result.
    /// </summary>
    /// <param name="parameter">The parameter.</param>
    /// <param name="packet">The packet.</param>
    /// <param name="inlineTypeConverter">The inline converter generator.</param>
    public void DeserializeAndCheck(ParameterInfo parameter, PacketInfo packet, InlineTypeConverterGenerator inlineTypeConverter)
    {
        _textWriter.WriteLine($"var {parameter.GetResultVariableName()} = ");
        inlineTypeConverter.CallDeserialize(_textWriter, packet);

        _textWriter.WriteLine($"if (!{parameter.GetResultVariableName()}.IsSuccess)");
        _textWriter.Indent++;
        _textWriter.WriteLine
        (
            $"return Result<{packet.Name}?>.FromError(new PacketParameterSerializerError(this, \"{parameter.Name}\", {parameter.GetResultVariableName()}), {parameter.GetResultVariableName()});"
        );
        _textWriter.Indent--;

        _textWriter.WriteLine
        (
            $"{parameter.GetNullableType()} {parameter.GetNullableVariableName()} = {parameter.GetResultVariableName()}.Entity;"
        );
    }
}