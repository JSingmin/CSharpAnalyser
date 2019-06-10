using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CSharpAnalyser.Lib.Resolvers
{
    public static class PredefinedTypeResolver
    {
        private static IReadOnlyDictionary<string, Type> PredefinedTypes { get; } = new Dictionary<string, Type>
        {
            { "bool", typeof(bool) },
            { "byte", typeof(byte) },
            { "char", typeof(char) },
            { "decimal", typeof(decimal) },
            { "double", typeof(double) },
            { "int", typeof(int) },
            { "object", typeof(object) },
            { "string", typeof(string) },
            { "void", typeof(void) },
        };

        public static Type GetNodeDataType(SyntaxNode node)
        {
            PredefinedTypeSyntax castedNode = (PredefinedTypeSyntax)node;
            string value = castedNode.Keyword.Text;
            if (!PredefinedTypes.ContainsKey(value)) throw new NotSupportedException($"PredefinedType {value} not catered for");

            return PredefinedTypes[value];
        }
    }
}
