using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using CSharpAnalyser.Lib.Utilities;

namespace CSharpAnalyser.Lib.Resolvers
{
    public static class IdentifierNameResolver
    {
        public static Type GetNodeDataType(SyntaxNode node)
        {
            IdentifierNameSyntax castedNode = (IdentifierNameSyntax)node;
            SyntaxNode declarationNode = IdentifierDeclarationFinder.Find(castedNode);

            // IdentifierDeclarationFinder will throw exception if it can't find the declaration,
            // so don't need to check and throw here
            return TypeResolver.GetNodeDataType(declarationNode);
        }
    }
}
