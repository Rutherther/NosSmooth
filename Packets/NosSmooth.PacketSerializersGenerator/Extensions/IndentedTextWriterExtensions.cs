//
//  IndentedTextWriterExtensions.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.CodeDom.Compiler;

namespace NosSmooth.PacketSerializersGenerator.Extensions;

/// <summary>
/// Extension methods for <see cref="IndentedTextWriter"/>.
/// </summary>
public static class IndentedTextWriterExtensions
{
    /// <summary>
    /// Append multiline text with correct indentation.
    /// </summary>
    /// <param name="textWriter">The text writer to write to.</param>
    /// <param name="text">The text to write.</param>
    public static void WriteMultiline(this IndentedTextWriter textWriter, string text)
    {
        foreach (var line in text.Split('\n'))
        {
            textWriter.WriteLine(line);
        }
    }
}