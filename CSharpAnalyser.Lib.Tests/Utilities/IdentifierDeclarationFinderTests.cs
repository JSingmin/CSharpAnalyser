using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Xunit;
using CSharpAnalyser.Lib.Utilities;

namespace CSharpAnalyser.Lib.Tests.Utilities
{
    public sealed class IdentifierDeclarationFinderTests
    {
        [Fact]
        public void FindsDeclarationWithinSameBlock()
        {
            string code = @"
            using System;

            public class TestClass
            {
                public void TestMethod()
                {
                    int testInt = 1;
                    var testInt2 = testInt;
                }
            }
            ";

            CompilationUnitSyntax root = CSharpSyntaxTree.ParseText(code).GetCompilationUnitRoot();
            IdentifierNameSyntax variableNode = root.DescendantNodes()
                .OfType<IdentifierNameSyntax>()
                .Where(n => n.Identifier.Text == "testInt")
                .Last();

            SyntaxNode declaration = IdentifierDeclarationFinder.Find(variableNode);

            Assert.NotNull(declaration);
            Assert.Equal("int testInt = 1;", declaration.ToString());
        }

        [Fact]
        public void FindsDeclarationInMethodParameter()
        {
            string code = @"
            using System;

            public class TestClass
            {
                public void testMethod(int testInt)
                {
                    var testInt2 = testInt;
                }
            }
            ";

            CompilationUnitSyntax root = CSharpSyntaxTree.ParseText(code).GetCompilationUnitRoot();
            IdentifierNameSyntax variableNode = root.DescendantNodes()
                .OfType<IdentifierNameSyntax>()
                .Where(n => n.Identifier.Text == "testInt")
                .Last();

            SyntaxNode declaration = IdentifierDeclarationFinder.Find(variableNode);

            Assert.NotNull(declaration);
            Assert.Equal("int testInt", declaration.ToString());
        }

        [Fact]
        public void FindsFieldDeclarationInClass()
        {
            string code = @"
            using System;

            public class TestClass
            {
                private int testInt = 1;

                public void TestMethod()
                {
                    var testInt2 = testInt;
                }
            }
            ";

            CompilationUnitSyntax root = CSharpSyntaxTree.ParseText(code).GetCompilationUnitRoot();
            IdentifierNameSyntax variableNode = root.DescendantNodes()
                .OfType<IdentifierNameSyntax>()
                .Where(n => n.Identifier.Text == "testInt")
                .Last();

            SyntaxNode declaration = IdentifierDeclarationFinder.Find(variableNode);

            Assert.NotNull(declaration);
            Assert.Equal("private int testInt = 1;", declaration.ToString());
        }

        [Fact]
        public void FindsMethodDeclarationInClass()
        {
            string code = @"
            using System;

            public class TestClass
            {
                public void TestMethod()
                {
                    var testInt2 = GetTestInt();
                }

                private int GetTestInt()
                {
                    return 0;
                }
            }
            ";

            CompilationUnitSyntax root = CSharpSyntaxTree.ParseText(code).GetCompilationUnitRoot();
            IdentifierNameSyntax variableNode = root.DescendantNodes()
                .OfType<IdentifierNameSyntax>()
                .Where(n => n.Identifier.Text == "GetTestInt")
                .First();

            SyntaxNode declaration = IdentifierDeclarationFinder.Find(variableNode);

            Assert.NotNull(declaration);
            Assert.StartsWith("private int GetTestInt()", declaration.ToString());
        }
    }
}
