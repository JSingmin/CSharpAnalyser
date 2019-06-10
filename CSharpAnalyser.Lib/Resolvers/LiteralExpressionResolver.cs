using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CSharpAnalyser.Lib.Resolvers
{
    public static class LiteralExpressionResolver
    {
        public static Type GetNodeDataType(SyntaxNode node)
        {
            LiteralExpressionSyntax castedNode = (LiteralExpressionSyntax)node;
            return castedNode.Token.Value.GetType();
        }
    }
}
