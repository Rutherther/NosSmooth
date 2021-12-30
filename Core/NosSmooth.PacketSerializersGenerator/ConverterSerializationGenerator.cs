//
//  ConverterSerializationGenerator.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.CodeDom.Compiler;
using NosSmooth.PacketSerializersGenerator.Data;
using NosSmooth.PacketSerializersGenerator.Extensions;

namespace NosSmooth.PacketSerializersGenerator;

/// <summary>
/// Various templates for converter serialization.
/// </summary>
public class ConverterSerializationGenerator
{
    private readonly string _builderVariable = "builder";
    private readonly IndentedTextWriter _textWriter;

    /// <summary>
    /// Initializes a new instance of the <see cref="ConverterSerializationGenerator"/> class.
    /// </summary>
    /// <param name="textWriter">The text writer.</param>
    public ConverterSerializationGenerator(IndentedTextWriter textWriter)
    {
        _textWriter = textWriter;
    }

    /// <summary>
    /// Push level to the string enumerator.
    /// </summary>
    /// <param name="separator">The separator.</param>
    public void SetAfterSeparatorOnce(char separator)
    {
        _textWriter.WriteLine(@$"{_builderVariable}.SetAfterSeparatorOnce(""{separator}"");");
    }

    /// <summary>
    /// Push level to the string enumerator.
    /// </summary>
    /// <param name="separator">The separator.</param>
    public void PushLevel(char separator)
    {
        _textWriter.WriteLine
            (@$"{_builderVariable}.PushLevel(""{separator}"");");
    }

    /// <summary>
    /// Pop level from the string enumerator.
    /// </summary>
    public void PopLevel()
    {
        _textWriter.WriteLine($"{_builderVariable}.PopLevel();");
    }

    /// <summary>
    /// Prepare the level to the string enumerator.
    /// </summary>
    /// <param name="separator">The separator.</param>
    /// <param name="maxTokens">The maximum number of tokens to read.</param>
    public void PrepareLevel(char separator, uint? maxTokens = default)
    {
        _textWriter.WriteLine
            ($@"{_builderVariable}.PrepareLevel(""{separator}"", {maxTokens?.ToString() ?? "null"});");
    }

    /// <summary>
    /// Prepare the level to the string enumerator.
    /// </summary>
    public void RemovePreparedLevel()
    {
        _textWriter.WriteLine($@"{_builderVariable}.RemovePreparedLevel();");
    }

    /// <summary>
    /// Deserialize the given parameter and check the result.
    /// </summary>
    /// <param name="parameter">The parameter to deserialize.</param>
    public void SerializeAndCheck(ParameterInfo parameter)
    {
        _textWriter.WriteMultiline
        (
            $@"
var {parameter.GetResultVariableName()} = _typeConverterRepository.Serialize<{parameter.GetActualType()}>(obj.{parameter.Name}, {_builderVariable});
if (!{parameter.GetResultVariableName()}.IsSuccess)
{{
    return Result.FromError(new PacketParameterSerializerError(this, ""{parameter.Name}"", {parameter.GetResultVariableName()}), {parameter.GetResultVariableName()});
}}
"
        );
    }
}