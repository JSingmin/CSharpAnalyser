using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CSharpAnalyser.Lib.Utilities
{
    public static class ConcatinationUtilities
    {
        private static IReadOnlyDictionary<Type, Func<SyntaxNode, SyntaxNode>> ConcatinationNodeFinders { get; } = new Dictionary<Type, Func<SyntaxNode, SyntaxNode>>
        {
            // Assumption: other node types not specified in this dictionary are safe
            // Limitation: does not cater for concatination methods (e.g. string.Format, StringBuilder)
            { typeof(ArgumentSyntax), FindConcatinatedArgumentSyntaxNode },
            { typeof(BinaryExpressionSyntax), FindConcatinatedBinaryExpressionSyntaxNode },
            { typeof(FieldDeclarationSyntax), FindConcatinatedFieldDeclarationSyntaxNode },
            { typeof(IdentifierNameSyntax), FindConcatinatedIdentifierNameSyntaxNode },
            { typeof(LocalDeclarationStatementSyntax), FindConcatinatedLocalDeclarationStatementSyntaxNode }
        };

        public static SyntaxNode GetConcatinatedNode(SyntaxNode node)
        {
            // Given a Syntax node, find if it contains a concatination, and returns where the concatination occurs.
            // A null return value indicates that no concatination could be found
            if (node == null) return null;

            Type nodeType = node.GetType();
            if (ConcatinationNodeFinders.ContainsKey(nodeType))
            {
                return ConcatinationNodeFinders[nodeType].Invoke(node);
            }

            return null;
        }

        public static bool IsNodeValueConcatinated(SyntaxNode node) => GetConcatinatedNode(node) != null;

        public static bool IsSafeValueConcatination(BinaryExpressionSyntax node, IReadOnlyDictionary<Type, IReadOnlyCollection<Type>> safeConcatinations)
        {
            Type left = TypeResolver.GetNodeDataType(node.Left);
            Type right = TypeResolver.GetNodeDataType(node.Right);

            return safeConcatinations.ContainsKey(left) && safeConcatinations[left].Contains(right);
        }

        private static SyntaxNode FindConcatinatedArgumentSyntaxNode(SyntaxNode argumentNode)
        {
            ArgumentSyntax castedNode = (ArgumentSyntax)argumentNode;
            return GetConcatinatedNode(castedNode.Expression);
        }

        private static SyntaxNode FindConcatinatedBinaryExpressionSyntaxNode(SyntaxNode expressionNode)
        {
            return expressionNode.Kind() == SyntaxKind.AddExpression
                ? expressionNode
                : null;
        }

        private static SyntaxNode FindConcatinatedFieldDeclarationSyntaxNode(SyntaxNode declarationNode)
        {
            // Limitation: assumes only one variable per declaration
            FieldDeclarationSyntax castedNode = (FieldDeclarationSyntax)declarationNode;
            ExpressionSyntax declaredValueNode = castedNode.Declaration.Variables.FirstOrDefault()?.Initializer?.Value;
            return declaredValueNode != null
                ? GetConcatinatedNode(declaredValueNode)
                : null;
        }

        private static SyntaxNode FindConcatinatedIdentifierNameSyntaxNode(SyntaxNode identifierNode)
        {
            IdentifierNameSyntax castedNode = (IdentifierNameSyntax)identifierNode;
            try
            {
                SyntaxNode declaration = IdentifierDeclarationFinder.Find(castedNode);
                return GetConcatinatedNode(declaration);
            }
            catch(InvalidOperationException)
            {
                // variable declaration couldn't be found, return that it isn't concatinated to reduce potential error reporting noise
            }

            return null;
        }

        private static SyntaxNode FindConcatinatedLocalDeclarationStatementSyntaxNode(SyntaxNode declarationNode)
        {
            // Limitation: assumes only one variable per declaration
            LocalDeclarationStatementSyntax castedNode = (LocalDeclarationStatementSyntax)declarationNode;
            ExpressionSyntax declaredValueNode = castedNode.Declaration.Variables.FirstOrDefault()?.Initializer?.Value;
            return declaredValueNode != null
                ? GetConcatinatedNode(declaredValueNode)
                : null;
        }
    }
}
