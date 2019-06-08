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
    public static class ParameterResolver
    {
        public static Type GetNodeDataType(SyntaxNode node)
        {
            ParameterSyntax castedNode = (ParameterSyntax)node;
            return TypeResolver.GetNodeDataType(node.ChildNodes().First());
        }
    }
}
