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
    public class UnusedMethodAnalyser : CSharpSyntaxWalker, IAnalyser
    {
        // Assumptions:
        // - All methods are public (accessible to all other classes)
        // - Classes are within the same namespace
        // - No nested classes
        // Limitations:
        // - Does not type-match parameters/arguments
        // - Does not cater for optional parameters/arguments

        private static string ReporterMessage { get; } = "Unused method";
        private List<MethodDeclarationHolder> MethodDeclarations { get; set; } = new List<MethodDeclarationHolder>();
        private List<MethodCallHolder> MethodCalls { get; set; } = new List<MethodCallHolder>();

        public IReadOnlyCollection<AnalyserItem> AnalyserItems => this.MethodDeclarations
            .Where(d => !d.IsReferenced(this.MethodCalls))
            .Select(d => new AnalyserItem(ReporterMessage, d.NodeReference))
            .ToList()
            .AsReadOnly();

        public override void VisitInvocationExpression(InvocationExpressionSyntax node)
        {
            IEnumerable<IdentifierNameSyntax> objectAndMethod = node.Expression.DescendantNodes()
                .OfType<IdentifierNameSyntax>()
                .TakeLast(2);

            if (objectAndMethod.Count() == 2)
            {
                // Assumption: for this analyser, assuming that variables are declared with a type (not var/base class/interface)
                IdentifierNameSyntax classIdentifier = objectAndMethod.First();
                SyntaxNode declaration = IdentifierDeclarationFinder.Find(classIdentifier);
                string className = declaration != null
                    ? declaration.ChildNodes().OfType<VariableDeclarationSyntax>().FirstOrDefault()?.Type.ToString()
                    // no declaration was found, assume static method
                    : classIdentifier.Identifier.Text;

                MethodCalls.Add(new MethodCallHolder(className, node));
            } else {
                string className = node.Ancestors()
                    .OfType<ClassDeclarationSyntax>()
                    .FirstOrDefault()?
                    .Identifier
                    .Text;

                MethodCalls.Add(new MethodCallHolder(className, node));
            }

            base.VisitInvocationExpression(node);
        }

        public override void VisitClassDeclaration(ClassDeclarationSyntax node)
        {
            string className = node.Identifier.Text;

            this.MethodDeclarations.AddRange(node.Members
                .OfType<MethodDeclarationSyntax>()
                .Select(m => new MethodDeclarationHolder(className, m))
            );

            base.VisitClassDeclaration(node);
        }

        private class MethodDeclarationHolder
        {
            public MethodDeclarationHolder(string className, MethodDeclarationSyntax node)
            {
                this.ClassName = className;
                this.MethodName = node.Identifier.Text;
                this.ParameterCount = node.ParameterList.Parameters.Count;
                this.NodeReference = node.GetReference();
            }

            public string ClassName { get; }

            public string MethodName { get; }

            public int ParameterCount { get; }

            public SyntaxReference NodeReference { get; }

            public bool IsReferenced(List<MethodCallHolder> methodCalls)
            {
                return methodCalls.Any(c =>
                    string.Equals(this.ClassName, c.ClassName) &&
                    string.Equals(this.MethodName, c.MethodName) &&
                    this.ParameterCount == c.ArgumentCount
                );
            }
        }

        private class MethodCallHolder
        {
            public MethodCallHolder(string className, InvocationExpressionSyntax node)
            {
                this.ClassName = className;
                this.MethodName = node.Expression.DescendantNodesAndSelf()
                    .OfType<IdentifierNameSyntax>()
                    .LastOrDefault()?
                    .Identifier
                    .Text;
                this.ArgumentCount = node.ArgumentList.Arguments.Count;
            }

            public string ClassName { get; }

            public string MethodName { get; }

            public int ArgumentCount { get; }
        }
    }
}
