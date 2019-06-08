using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CSharpAnalyser.Lib.Models
{
    public class AnalyserItem
    {
        public AnalyserItem(string message, SyntaxReference nodeReference)
        {
            this.Message = message;
            this.NodeReference = nodeReference;
        }

        public string Message { get; }

        public SyntaxReference NodeReference { get; }
    }
}
