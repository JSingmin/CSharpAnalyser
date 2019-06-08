using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CSharpAnalyser.Lib.Resolvers
{
    public static class LiteralExpressionResolver
    {
        private static IReadOnlyDictionary<SyntaxKind, Func<LiteralExpressionSyntax, Type>> LiteralResolvers { get; } = new Dictionary<SyntaxKind, Func<LiteralExpressionSyntax, Type>>
        {
            { SyntaxKind.CharacterLiteralExpression, _ => typeof(char) },
            { SyntaxKind.FalseLiteralExpression, _ => typeof(bool) },
            { SyntaxKind.NullLiteralExpression, _ => typeof(object) },
            { SyntaxKind.NumericLiteralExpression, ResolveNumeric },
            { SyntaxKind.StringLiteralExpression, _ => typeof(string) },
            { SyntaxKind.TrueLiteralExpression, _ => typeof(bool) },
        };

        public static Type GetNodeDataType(SyntaxNode node)
        {
            LiteralExpressionSyntax castedNode = (LiteralExpressionSyntax)node;
            return castedNode.Token.Value.GetType();
            /*
            LiteralExpressionSyntax castedNode = (LiteralExpressionSyntax)node;
            SyntaxKind nodeKind = castedNode.Kind();
            if (!LiteralResolvers.ContainsKey(nodeKind)) throw new NotSupportedException($"SyntaxKind {nodeKind} not catered for");

            return LiteralResolvers[nodeKind].Invoke(castedNode);
             */
        }

        private static Type ResolveNumeric(LiteralExpressionSyntax node)
        {
            //if (byte.TryParse(node.Token.))

            return typeof(double);
        }
    }
}
