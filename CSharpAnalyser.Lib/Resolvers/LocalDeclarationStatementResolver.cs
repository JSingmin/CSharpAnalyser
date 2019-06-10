using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using CSharpAnalyser.Lib.Utilities;

namespace CSharpAnalyser.Lib.Resolvers
{
    public static class LocalDeclarationStatementResolver
    {
        public static Type GetNodeDataType(SyntaxNode node)
        {
            LocalDeclarationStatementSyntax castedNode = (LocalDeclarationStatementSyntax)node;

            TypeSyntax typeDeclaration = castedNode.Declaration.Type;
            if (!typeDeclaration.IsVar)
            {
                return TypeResolver.GetNodeDataType(typeDeclaration);
            }

            var initializer = castedNode.Declaration.Variables.FirstOrDefault()?.Initializer.Value;
            if (initializer != null)
            {
                return TypeResolver.GetNodeDataType(initializer);
            }

            throw new InvalidOperationException("Variable declared with var keyword and has no definition");
        }
    }
}
