using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using CSharpAnalyser.Lib.Utilities;

namespace CSharpAnalyser.Lib.Resolvers
{
    public static class MethodDeclarationResolver
    {
        public static Type GetNodeDataType(SyntaxNode node)
        {
            MethodDeclarationSyntax castedNode = (MethodDeclarationSyntax)node;

            return TypeResolver.GetNodeDataType(castedNode.ReturnType);
        }
    }
}
