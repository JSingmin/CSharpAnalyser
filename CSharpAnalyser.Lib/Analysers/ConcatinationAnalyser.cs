using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using CSharpAnalyser.Lib.Services;

namespace CSharpAnalyser.Lib.Analysers
{
    public class ConcatinationAnalyser : CSharpSyntaxWalker
    {
        public override void VisitBinaryExpression(BinaryExpressionSyntax node)
        {
            if (node.Kind() == SyntaxKind.AddExpression && !IsConcatinationSafe(node))
            {
                var nodeLocation = node.GetLocation().GetMappedLineSpan();
                Console.WriteLine($"Unsafe concatination detected: {node.ToFullString()}, {nodeLocation.Path}:{nodeLocation.StartLinePosition.Line}");
            }

            base.VisitBinaryExpression(node);
        }

        private static bool IsConcatinationSafe(SyntaxNode node)
        {
            var childNodeTypes = new List<Type>();
            foreach (var n in node.ChildNodes())
            {
                if (!IsConcatinationSafe(n)) return false;
                //childNodeTypes.Add(TypeResolverService.GetNodeDataType(n));
            }

            return childNodeTypes.Distinct().Count() <= 1;
        }
    }
}
