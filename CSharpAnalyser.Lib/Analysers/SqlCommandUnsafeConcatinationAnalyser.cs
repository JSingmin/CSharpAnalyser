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
    public class SqlCommandUnsafeConcatinationAnalyser : CSharpSyntaxWalker, IAnalyser
    {
        private static string SqlCommandClassName { get; } = "SqlCommand";
        private static string ReporterMessage { get; } = "Concatinated SQL string is unsafe";
        private static IReadOnlyDictionary<Type, IReadOnlyCollection<Type>> SafeConcatinationTypes { get; } = new Dictionary<Type, IReadOnlyCollection<Type>>
        {
            { typeof(string), new List<Type>{ typeof(decimal), typeof(double), typeof(int) }.AsReadOnly() }
        };

        private List<AnalyserItem> ReportableItems { get; } = new List<AnalyserItem>();

        public IReadOnlyCollection<AnalyserItem> AnalyserItems => this.ReportableItems.AsReadOnly();

        public override void VisitObjectCreationExpression(ObjectCreationExpressionSyntax node)
        {
            if (IsSqlCommandCreatorNode(node))
            {
                SyntaxNode concatenatedNode = GetConcatinatedCommandTextNode(node.ArgumentList);
                if (concatenatedNode != null && !ConcatinationUtilities.IsSafeValueConcatination(concatenatedNode, SafeConcatinationTypes))
                {
                    Console.WriteLine($"{concatenatedNode.GetType()}|{concatenatedNode.Kind()}|{concatenatedNode.ToString()}");
                    this.ReportableItems.Add(new AnalyserItem(ReporterMessage, node.GetReference()));
                }
            }

            base.VisitObjectCreationExpression(node);
        }

        private static bool IsSqlCommandCreatorNode(ObjectCreationExpressionSyntax node)
        {
            return node.Initializer
                .DescendantNodesAndSelf()
                .OfType<IdentifierNameSyntax>()
                .Any(n => string.Equals(n.Identifier.Text, SqlCommandClassName));
        }

        private static SyntaxNode GetConcatinatedCommandTextNode(ArgumentListSyntax arguments)
        {
            // Get the first argument syntax node, which will be the SqlCommand's command text
            // Limitation: does not cater for named parameters, which may change the ordinal position of arguments
            SyntaxNode commandTextArgumentNode = arguments.Arguments.First().Expression;
            return ConcatinationUtilities.GetConcatinatedNode(commandTextArgumentNode);
        }
    }
}
