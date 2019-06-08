using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CSharpAnalyser.Lib.Analysers
{
    public class UnusedMethodAnalyser : CSharpSyntaxWalker
    {
        public override void VisitInvocationExpression(InvocationExpressionSyntax node)
        {
            Console.WriteLine(node.ToFullString());

            base.VisitInvocationExpression(node);
        }

        public override void VisitMethodDeclaration(MethodDeclarationSyntax node)
        {
            Console.WriteLine(node.ToFullString());

            base.VisitMethodDeclaration(node);
        }
    }
}
