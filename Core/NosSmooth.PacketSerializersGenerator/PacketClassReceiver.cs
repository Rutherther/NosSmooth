//
//  PacketClassReceiver.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace NosSmooth.PacketSerializersGenerator
{
    /// <summary>
    /// Syntax receiver of classes with generate attribute.
    /// </summary>
    public class PacketClassReceiver : ISyntaxReceiver
    {
        private readonly List<RecordDeclarationSyntax> _packetClasses;

        /// <summary>
        /// Gets the name of the attribute that indicates the packet should have a serializer generated.
        /// </summary>
        public static string AttributeFullName => "GenerateSerializer";

        /// <summary>
        /// Initializes a new instance of the <see cref="PacketClassReceiver"/> class.
        /// </summary>
        public PacketClassReceiver()
        {
            _packetClasses = new List<RecordDeclarationSyntax>();
        }

        /// <summary>
        /// Gets the classes that should have serializers generated.
        /// </summary>
        public IReadOnlyList<RecordDeclarationSyntax> PacketClasses => _packetClasses;

        /// <inheritdoc />
        public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
        {
            if (syntaxNode.IsKind(SyntaxKind.RecordDeclaration) && syntaxNode is RecordDeclarationSyntax classDeclarationSyntax)
            {
                if (classDeclarationSyntax.AttributeLists.Any(x => x.Attributes.Any(x => x.Name.NormalizeWhitespace().ToFullString() == AttributeFullName)))
                {
                    _packetClasses.Add(classDeclarationSyntax);
                }
            }
        }
    }
}