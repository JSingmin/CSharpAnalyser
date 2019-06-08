using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using CSharpAnalyser.Lib.Models;
using CSharpAnalyser.Lib.Services;
using CSharpAnalyser.Lib.Utilities;

namespace CSharpAnalyser.Lib.Analysers
{
    public class SqlCommandConcatinationAnalyser : CSharpSyntaxWalker, IAnalyser
    {
        private static string SqlCommandClassName { get; } = "SqlCommand";
        private static string ReporterMessage { get; } = "Concatinated SQL string";

        /*
        private static IReadOnlyDictionary<Type, Func<SyntaxNode, bool>> ArgumentNodeTypeCheckers { get; } = new Dictionary<Type, Func<SyntaxNode, bool>>
        {
            // Assumption: other node types not specified in this dictionary are safe
            // Limitation: does not cater for concatination methods (e.g. string.Format, StringBuilder)
            { typeof(BinaryExpressionSyntax), IsBinaryExpressionSyntaxNodeConcatinated },
            { typeof(FieldDeclarationSyntax), IsFieldDeclarationSyntaxConcatinated },
            { typeof(IdentifierNameSyntax), IsIdentifierNameSyntaxConcatinated },
            { typeof(LocalDeclarationStatementSyntax), IsLocalDeclarationStatementSyntaxConcatinated }
        };
        */

        private List<AnalyserItem> ReportableItems { get; } = new List<AnalyserItem>();

        public IReadOnlyCollection<AnalyserItem> AnalyserItems => this.ReportableItems.AsReadOnly();

        public override void VisitObjectCreationExpression(ObjectCreationExpressionSyntax node)
        {
            if (IsSqlCommandCreatorNode(node) && IsConcatinatedCommandText(node))
            {
                this.ReportableItems.Add(new AnalyserItem(ReporterMessage, node.GetReference()));
            }

            base.VisitObjectCreationExpression(node);
        }

        private static bool IsSqlCommandCreatorNode(SyntaxNode node)
        {
            return node.ChildNodes().First()
                .DescendantNodesAndSelf()
                .OfType<IdentifierNameSyntax>()
                .Any(n => string.Equals(n.Identifier.Text, SqlCommandClassName));
        }

        private static bool IsConcatinatedCommandText(SyntaxNode node)
        {
            // Get the first argument syntax node, which will be the SqlCommand's command text
            // Limitation: does not cater for named parameters, which may change the ordinal position of arguments
            SyntaxNode commandTextArgumentNode = node.DescendantNodes()
                .OfType<ArgumentSyntax>()
                .First()
                .ChildNodes()
                .First();

            return ConcatinationUtilities.IsNodeValueConcatinated(commandTextArgumentNode);
        }

        /*
        private static bool IsBinaryExpressionSyntaxNodeConcatinated(SyntaxNode expressionNode)
        {
            return expressionNode.Kind() == SyntaxKind.AddExpression;
        }

        private static bool IsFieldDeclarationSyntaxConcatinated(SyntaxNode declarationNode)
        {
            // Limitation: assumes only one variable per declaration
            FieldDeclarationSyntax castedNode = (FieldDeclarationSyntax)declarationNode;
            ExpressionSyntax declaredValueNode = castedNode.Declaration.Variables.First()?.Initializer?.Value;
            if (declaredValueNode == null)
            {
                return false;
            }

            return IsNodeValueConcatinated(declaredValueNode);
        }

        private static bool IsIdentifierNameSyntaxConcatinated(SyntaxNode identifierNode)
        {
            IdentifierNameSyntax castedNode = (IdentifierNameSyntax)identifierNode;
            try
            {
                SyntaxNode declaration = IdentifierDeclarationFinder.Find(castedNode);
                return IsNodeValueConcatinated(declaration);
            }
            catch(InvalidOperationException)
            {
                // variable declaration couldn't be found, return that it isn't concatinated to reduce potential error reporting noise
            }

            return false;
        }

        private static bool IsLocalDeclarationStatementSyntaxConcatinated(SyntaxNode declarationNode)
        {
            // Limitation: assumes only one variable per declaration
            LocalDeclarationStatementSyntax castedNode = (LocalDeclarationStatementSyntax)declarationNode;
            ExpressionSyntax declaredValueNode = castedNode.Declaration.Variables.First()?.Initializer?.Value;
            if (declaredValueNode == null)
            {
                return false;
            }

            return IsNodeValueConcatinated(declaredValueNode);
        }

        private static bool IsNodeValueConcatinated(SyntaxNode node)
        {
            Type nodeType = node.GetType();
            if (ArgumentNodeTypeCheckers.ContainsKey(nodeType))
            {
                return ArgumentNodeTypeCheckers[nodeType].Invoke(node);
            }

            return false;
        }
        */
    }
}
