//
//  DiagnosticError.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace NosSmooth.PacketSerializersGenerator.Errors;

/// <summary>
/// The diagnostic error.
/// </summary>
/// <param name="Id"></param>
/// <param name="Title"></param>
/// <param name="MessageFormat"></param>
/// <param name="Tree"></param>
/// <param name="Span"></param>
/// <param name="Parameters"></param>
public record DiagnosticError(string Id, string Title, string MessageFormat, SyntaxTree Tree, TextSpan Span, List<object?> Parameters) : IError
{
    /// <inheritdoc />
    public string Message => Title;
}