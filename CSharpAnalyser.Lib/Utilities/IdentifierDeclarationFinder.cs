using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CSharpAnalyser.Lib.Utilities
{
    public static class IdentifierDeclarationFinder
    {
        private static IReadOnlyDictionary<Type, Func<string, SyntaxNode, SyntaxNode>> NodeFinders { get; } = new Dictionary<Type, Func<string, SyntaxNode, SyntaxNode>>
        {
            { typeof(BlockSyntax), SearchBlock },
            { typeof(ClassDeclarationSyntax), SearchClassDeclaration },
            { typeof(MethodDeclarationSyntax), SearchMethodDeclaration }
        };

        public static SyntaxNode Find(IdentifierNameSyntax identifierNode)
        {
            // Given an identifier node, find the identifier's originating declaration
            // Limitation: This method only searches the given SyntaxTree (won't search across files)

            if (identifierNode == null) throw new ArgumentNullException(nameof(identifierNode));

            string identifierName = identifierNode.Identifier.Text;
            SyntaxNode examiningNode = identifierNode.Parent;
            while (examiningNode != null)
            {
                if (NodeFinders.ContainsKey(examiningNode.GetType()))
                {
                    var potentialDeclaration = NodeFinders[examiningNode.GetType()].Invoke(identifierName, examiningNode);
                    if (potentialDeclaration != null)
                    {
                        //Console.WriteLine($"{potentialDeclaration.ToString()}|{TypeResolver.GetNodeDataType(potentialDeclaration).ToString()}");
                        return potentialDeclaration;
                    }
                }

                examiningNode = examiningNode.Parent;
            }

            throw new InvalidOperationException($"Identifer {identifierName} not found");
        }

        private static SyntaxNode SearchBlock(string identifierName, SyntaxNode node)
        {
            BlockSyntax castedNode = (BlockSyntax)node;
            return castedNode.Statements
                .OfType<LocalDeclarationStatementSyntax>()
                .FirstOrDefault(n => n.Declaration.Variables.Any(v => string.Equals(v.Identifier.Text, identifierName)));
        }
        private static SyntaxNode SearchClassDeclaration(string identifierName, SyntaxNode node)
        {
            ClassDeclarationSyntax castedNode = (ClassDeclarationSyntax)node;
            SyntaxNode variable = castedNode.Members
                .OfType<FieldDeclarationSyntax>()
                .FirstOrDefault(f => f.Declaration.Variables.Any(v => string.Equals(v.Identifier.Text, identifierName)));

            if (variable != null) return variable;

            return castedNode.Members
                .OfType<MethodDeclarationSyntax>()
                .FirstOrDefault(m => string.Equals(m.Identifier.Text, identifierName));
        }

        private static SyntaxNode SearchMethodDeclaration(string identifierName, SyntaxNode node)
        {
            MethodDeclarationSyntax castedNode = (MethodDeclarationSyntax)node;
            return castedNode.ParameterList.Parameters.FirstOrDefault(p => string.Equals(p.Identifier.Text, identifierName));

            // string.Equals(castedNode.Identifier.Text, identifierName)
            //     ? examiningNode
            //     :
        }
    }
}
