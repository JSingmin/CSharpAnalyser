using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CSharpAnalyser.Lib.Parsers
{
    public static class StringParser
    {
        public static CompilationUnitSyntax Parse(string code)
        {
            SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(code);
            return syntaxTree.GetCompilationUnitRoot();
        }
    }
}
