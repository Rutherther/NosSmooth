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
        _textWriter.WriteLine(@$"{_builderVariable}.SetAfterSeparatorOnce('{separator}');");
    }

    /// <summary>
    /// Push level to the string enumerator.
    /// </summary>
    /// <param name="separator">The separator.</param>
    public void PushLevel(char separator)
    {
        _textWriter.WriteLine
            (@$"{_builderVariable}.PushLevel('{separator}');");
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
    public void PrepareLevel(char separator)
    {
        _textWriter.WriteLine
            ($@"{_builderVariable}.PrepareLevel('{separator}');");
    }

    /// <summary>
    /// Prepare the level to the string enumerator.
    /// </summary>
    public void RemovePreparedLevel()
    {
        _textWriter.WriteLine($@"{_builderVariable}.RemovePreparedLevel();");
    }
}