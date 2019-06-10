using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using CSharpAnalyser.Lib.Utilities;

namespace CSharpAnalyser.Lib.Resolvers
{
    public static class MemberAccessExpressionResolver
    {
        public static Type GetNodeDataType(SyntaxNode node)
        {
            MemberAccessExpressionSyntax castedNode = (MemberAccessExpressionSyntax)node;
            return TypeResolver.GetNodeDataType(castedNode.Name);
        }
    }
}
