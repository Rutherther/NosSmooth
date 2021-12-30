//
//  ConverterDeserializationGenerator.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.CodeDom.Compiler;
using NosSmooth.PacketSerializersGenerator.Data;
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
        _textWriter.WriteLine($"{_stringEnumeratorVariable}.GetNextToken();");
        _textWriter.Indent--;
        _textWriter.WriteLine("}");
    }

    /// <summary>
    /// Deserialize the given parameter and check the result.
    /// </summary>
    /// <param name="packetFullName">The full name of the packet.</param>
    /// <param name="parameter">The parameter to deserialize.</param>
    /// <param name="isLast">Whether the token is the last one.</param>
    public void DeserializeAndCheck(string packetFullName, ParameterInfo parameter, bool isLast)
    {
        string isLastString = isLast ? "true" : "false";
        _textWriter.WriteMultiline($@"
var {parameter.GetResultVariableName()} = _typeConverterRepository.Deserialize<{parameter.GetNullableType()}>({_stringEnumeratorVariable});
var {parameter.GetErrorVariableName()} = CheckDeserializationResult({parameter.GetResultVariableName()}, ""{parameter.Name}"", {_stringEnumeratorVariable}, {isLastString});
if ({parameter.GetErrorVariableName()} is not null)
{{
    return Result<{packetFullName}?>.FromError({parameter.GetErrorVariableName()}, {parameter.GetResultVariableName()});
}}
");
    }

    /// <summary>
    /// Check taht the given variable is not null, if it is, return an error.
    /// </summary>
    /// <param name="resultVariableName">The result variable to check for null.</param>
    /// <param name="parameterName">The parameter that is being parsed.</param>
    /// <param name="reason">The reason for the error.</param>
    public void CheckNullError(string resultVariableName, string parameterName, string reason = "The converter has returned null even though it was not expected.")
    {
        _textWriter.WriteMultiline($@"
if ({resultVariableName}.Entity is null) {{
    return new PacketParameterSerializerError(this, ""{parameterName}"", {resultVariableName}, ""{reason}"");
}}
");
    }

    /// <summary>
    /// Assign local variable with the result of the parameter deserialization.
    /// </summary>
    /// <param name="parameter">The parameter.</param>
    public void AssignLocalVariable(ParameterInfo parameter)
    {
        _textWriter.WriteLine($"var {parameter.Name} = ({parameter.GetActualType()}){parameter.GetResultVariableName()}.Entity;");
    }
}