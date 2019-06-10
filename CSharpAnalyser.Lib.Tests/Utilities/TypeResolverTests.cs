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
    public sealed class TypeResolverTests
    {
        [Fact]
        public void DeterminesVariableTypeByDeclaredType()
        {
            string code = @"
            using System;

            public class TestClass
            {
                public void TestMethod()
                {
                    int testInt;
                }
            }
            ";

            CompilationUnitSyntax root = CSharpSyntaxTree.ParseText(code).GetCompilationUnitRoot();
            LocalDeclarationStatementSyntax variableNode = root.DescendantNodes()
                .OfType<LocalDeclarationStatementSyntax>()
                .First();

            Type result = TypeResolver.GetNodeDataType(variableNode);

            Assert.Equal(typeof(int), result);
        }

        [Fact]
        public void DeterminesVariableTypeByDeclaredValue()
        {
            string code = @"
            using System;

            public class TestClass
            {
                public void TestMethod()
                {
                    var testInt = 1;
                }
            }
            ";

            CompilationUnitSyntax root = CSharpSyntaxTree.ParseText(code).GetCompilationUnitRoot();
            LocalDeclarationStatementSyntax variableNode = root.DescendantNodes()
                .OfType<LocalDeclarationStatementSyntax>()
                .First();

            Type result = TypeResolver.GetNodeDataType(variableNode);

            Assert.Equal(typeof(int), result);
        }

        [Fact]
        public void DeterminesVariableTypeFromAnotherVariable()
        {
            string code = @"
            using System;

            public class TestClass
            {
                public void TestMethod()
                {
                    var testInt1 = 1;
                    var testInt2 = testInt1;
                }
            }
            ";

            CompilationUnitSyntax root = CSharpSyntaxTree.ParseText(code).GetCompilationUnitRoot();
            LocalDeclarationStatementSyntax variableNode = root.DescendantNodes()
                .OfType<LocalDeclarationStatementSyntax>()
                .Last();

            Type result = TypeResolver.GetNodeDataType(variableNode);

            Assert.Equal(typeof(int), result);
        }

        [Fact]
        public void DeterminesTypeByMethodCall()
        {
            string code = @"
            using System;

            public class TestClass
            {
                public void TestMethod()
                {
                    this.GetInt();
                }

                private int GetInt()
                {
                    return 1;
                }
            }
            ";

            CompilationUnitSyntax root = CSharpSyntaxTree.ParseText(code).GetCompilationUnitRoot();
            InvocationExpressionSyntax node = root.DescendantNodes()
                .OfType<InvocationExpressionSyntax>()
                .First();

            Type result = TypeResolver.GetNodeDataType(node);

            Assert.Equal(typeof(int), result);
        }

        [Fact]
        public void DeterminesTypeByParameter()
        {
            string code = @"
            using System;

            public class TestClass
            {
                public void TestMethod(int parameter)
                {
                    var test = parameter;
                }
            }
            ";

            CompilationUnitSyntax root = CSharpSyntaxTree.ParseText(code).GetCompilationUnitRoot();
            LocalDeclarationStatementSyntax node = root.DescendantNodes()
                .OfType<LocalDeclarationStatementSyntax>()
                .First();

            Type result = TypeResolver.GetNodeDataType(node);

            Assert.Equal(typeof(int), result);
        }
    }
}
