using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using CSharpAnalyser.Lib.Models;

namespace CSharpAnalyser.Lib.Analysers
{
    public class WeakHashAnalyser : CSharpSyntaxWalker, IAnalyser
    {
        private static string ReporterMessage { get; } = "Weak hash algorithm usage detected";
        private static IReadOnlyCollection<string> WeakHashAlgorithms { get; } = new List<string>
        {
            "MD5"
        }.AsReadOnly();

        private List<AnalyserItem> ReportableItems { get; } = new List<AnalyserItem>();

        public IReadOnlyCollection<AnalyserItem> AnalyserItems => this.ReportableItems.AsReadOnly();

        public override void VisitInvocationExpression(InvocationExpressionSyntax node)
        {
            var detections = node.DescendantNodes()
                .OfType<IdentifierNameSyntax>()
                .Where(n => WeakHashAlgorithms.Any(a => n.Identifier.Text.IndexOf(a) >= 0));

            foreach (IdentifierNameSyntax identifier in detections)
            {
                this.ReportableItems.Add(new AnalyserItem(ReporterMessage, node.GetReference()));
            }

            base.VisitInvocationExpression(node);
        }
    }
}
