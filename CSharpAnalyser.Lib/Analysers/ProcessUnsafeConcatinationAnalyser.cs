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
    public class ProcessUnsafeConcatinationAnalyser : CSharpSyntaxWalker, IAnalyser
    {
        private static string ProcessClassName { get; } = "Process";
        private static string ProcessStartMethodName { get; } = "Start";
        private static string ReporterMessage { get; } = "Concatinated process start arguments are unsafe";
        private static IReadOnlyDictionary<Type, IReadOnlyCollection<Type>> SafeConcatinationTypes { get; } = new Dictionary<Type, IReadOnlyCollection<Type>>
        {
            { typeof(string), new List<Type>{ typeof(decimal), typeof(double), typeof(int) }.AsReadOnly() }
        };

        private List<AnalyserItem> ReportableItems { get; } = new List<AnalyserItem>();

        public IReadOnlyCollection<AnalyserItem> AnalyserItems => this.ReportableItems.AsReadOnly();

        public override void VisitInvocationExpression(InvocationExpressionSyntax node)
        {
            if (IsProcessStartNode(node))
            {
                SyntaxNode concatenatedNode = GetConcatinatedCommandArgumentsNode(node.ArgumentList);
                if (concatenatedNode != null && !ConcatinationUtilities.IsSafeValueConcatination(concatenatedNode, SafeConcatinationTypes))
                {
                    this.ReportableItems.Add(new AnalyserItem(ReporterMessage, node.GetReference()));
                }
            }

            base.VisitInvocationExpression(node);
        }

        private static bool IsProcessStartNode(InvocationExpressionSyntax node)
        {
            // Limitation: doesn't cater for delegates
            var processIdentifiers = node.Expression.DescendantNodes()
                .OfType<IdentifierNameSyntax>()
                .TakeLast(2);

            // Check that the last 2 identifiers are "Process" and "Start"
            return processIdentifiers.Count() == 2
                && string.Equals(processIdentifiers.First().Identifier.Text, ProcessClassName)
                && string.Equals(processIdentifiers.Last().Identifier.Text, ProcessStartMethodName);
        }

        private static SyntaxNode GetConcatinatedCommandArgumentsNode(ArgumentListSyntax node)
        {
            if (node.Arguments.Count <= 1) return null;

            // Get the first argument syntax node, which will be the SqlCommand's command text
            // Limitation: does not cater for named parameters, which may change the ordinal position of arguments
            SyntaxNode commandTextArgumentNode = node.Arguments.ElementAtOrDefault(1).Expression;
            return ConcatinationUtilities.GetConcatinatedNode(commandTextArgumentNode);
        }
    }
}
