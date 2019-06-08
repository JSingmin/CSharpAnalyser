using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using CSharpAnalyser.Lib.Resolvers;

namespace CSharpAnalyser.Lib.Utilities
{
    public static class TypeResolver
    {
        private static IReadOnlyDictionary<Type, Func<SyntaxNode, Type>> NodeDataTypeResolvers { get; } = new Dictionary<Type, Func<SyntaxNode, Type>>
        {
            { typeof(IdentifierNameSyntax), IdentifierNameResolver.GetNodeDataType },
            { typeof(InvocationExpressionSyntax), InvocationExpressionResolver.GetNodeDataType },
            { typeof(LiteralExpressionSyntax), LiteralExpressionResolver.GetNodeDataType },
            { typeof(LocalDeclarationStatementSyntax), LocalDeclarationStatementResolver.GetNodeDataType },
            { typeof(MemberAccessExpressionSyntax), MemberAccessExpressionResolver.GetNodeDataType },
            { typeof(MethodDeclarationSyntax), MethodDeclarationResolver.GetNodeDataType },
            { typeof(ParameterSyntax), ParameterResolver.GetNodeDataType },
            { typeof(PredefinedTypeSyntax), PredefinedTypeResolver.GetNodeDataType }
        };

        public static Type GetNodeDataType(SyntaxNode node)
        {
            Type nodeType = node.GetType();
            //if (!NodeDataTypeResolvers.ContainsKey(nodeType)) throw new NotSupportedException($"Node Type {nodeType}|{node.Kind()} not catered for");

            if (!NodeDataTypeResolvers.ContainsKey(nodeType)) return typeof(void);

            return NodeDataTypeResolvers[nodeType].Invoke(node);
        }
    }
}
