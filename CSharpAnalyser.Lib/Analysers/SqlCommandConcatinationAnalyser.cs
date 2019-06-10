using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using CSharpAnalyser.Lib.Models;
using CSharpAnalyser.Lib.Utilities;

namespace CSharpAnalyser.Lib.Analysers
{
    public class SqlCommandConcatinationAnalyser : CSharpSyntaxWalker, IAnalyser
    {
        private static string SqlCommandClassName { get; } = "SqlCommand";
        private static string ReporterMessage { get; } = "Concatinated SQL string";

        private List<AnalyserItem> ReportableItems { get; } = new List<AnalyserItem>();

        public IReadOnlyCollection<AnalyserItem> AnalyserItems => this.ReportableItems.AsReadOnly();

        public override void VisitObjectCreationExpression(ObjectCreationExpressionSyntax node)
        {
            if (IsSqlCommandCreatorNode(node) && IsConcatinatedCommandText(node.ArgumentList))
            {
                this.ReportableItems.Add(new AnalyserItem(ReporterMessage, node.GetReference()));
            }

            base.VisitObjectCreationExpression(node);
        }

        private static bool IsSqlCommandCreatorNode(ObjectCreationExpressionSyntax node)
        {
            return node.Type
                .DescendantNodesAndSelf()
                .OfType<IdentifierNameSyntax>()
                .Any(n => string.Equals(n.Identifier.Text, SqlCommandClassName));
        }

        private static bool IsConcatinatedCommandText(ArgumentListSyntax arguments)
        {
            // Get the first argument syntax node, which will be the SqlCommand's command text
            // Limitation: does not cater for named parameters, which may change the ordinal position of arguments
            SyntaxNode commandTextArgumentNode = arguments.Arguments.FirstOrDefault();
            return ConcatinationUtilities.IsNodeValueConcatinated(commandTextArgumentNode);
        }
    }
}
