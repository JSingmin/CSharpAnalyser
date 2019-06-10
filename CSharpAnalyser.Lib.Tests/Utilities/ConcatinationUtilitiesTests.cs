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
    public sealed class ConcatinationUtilitiesTests
    {
        [Fact]
        public void GetConcatinatedNodeReturnsNullIfArgumentIsNotConcatinated()
        {
            string code = @"
            using System;

            public class TestClass
            {
                public void TestMethod()
                {
                    System.Diagnostics.Process.Start(""CMD.exe"", ""echo test"");
                }
            }
            ";

            CompilationUnitSyntax root = CSharpSyntaxTree.ParseText(code).GetCompilationUnitRoot();
            ArgumentSyntax argument = root.DescendantNodes()
                .OfType<ArgumentListSyntax>()
                .Last()
                .Arguments
                .Last();

            SyntaxNode result = ConcatinationUtilities.GetConcatinatedNode(argument);

            Assert.Null(result);
        }

        [Fact]
        public void GetConcatinatedNodeReturnsNullIfArgumentIsMethodParameter()
        {
            string code = @"
            using System;

            public class TestClass
            {
                public void testMethod(string arguments)
                {
                    System.Diagnostics.Process.Start(""CMD.exe"", arguments);
                }
            }
            ";

            CompilationUnitSyntax root = CSharpSyntaxTree.ParseText(code).GetCompilationUnitRoot();
            ArgumentSyntax argument = root.DescendantNodes()
                .OfType<ArgumentListSyntax>()
                .Last()
                .Arguments
                .Last();

            SyntaxNode result = ConcatinationUtilities.GetConcatinatedNode(argument);

            Assert.Null(result);
        }

        [Fact]
        public void GetConcatinatedNodeFindsConcatinatedArgument()
        {
            string code = @"
            using System;

            public class TestClass
            {
                public void testMethod(string name)
                {
                    System.Diagnostics.Process.Start(""CMD.exe"", ""echo "" + name);
                }
            }
            ";

            CompilationUnitSyntax root = CSharpSyntaxTree.ParseText(code).GetCompilationUnitRoot();
            ArgumentSyntax argument = root.DescendantNodes()
                .OfType<ArgumentListSyntax>()
                .Last()
                .Arguments
                .Last();

            SyntaxNode result = ConcatinationUtilities.GetConcatinatedNode(argument);

            Assert.NotNull(result);
            Assert.Equal(@"""echo "" + name", result.ToString());
        }

        [Fact]
        public void GetConcatinatedNodeFindsConcatinatedVariableUsedAsArgument()
        {
            string code = @"
            using System;

            public class TestClass
            {
                public void testMethod(string name)
                {
                    string arguments = ""echo "" + name;
                    System.Diagnostics.Process.Start(""CMD.exe"", arguments);
                }
            }
            ";

            CompilationUnitSyntax root = CSharpSyntaxTree.ParseText(code).GetCompilationUnitRoot();
            ArgumentSyntax argument = root.DescendantNodes()
                .OfType<ArgumentListSyntax>()
                .Last()
                .Arguments
                .Last();

            SyntaxNode result = ConcatinationUtilities.GetConcatinatedNode(argument);

            Assert.NotNull(result);
            Assert.Equal(@"""echo "" + name", result.ToString());
        }

        [Fact]
        public void GetConcatinatedNodeFindsConcatinatedMaskedVariableUsedAsArgument()
        {
            string code = @"
            using System;

            public class TestClass
            {
                public void testMethod(string name)
                {
                    string arguments1 = ""echo "" + name;
                    string arguments2 = arguments1;
                    System.Diagnostics.Process.Start(""CMD.exe"", arguments2);
                }
            }
            ";

            CompilationUnitSyntax root = CSharpSyntaxTree.ParseText(code).GetCompilationUnitRoot();
            ArgumentSyntax argument = root.DescendantNodes()
                .OfType<ArgumentListSyntax>()
                .Last()
                .Arguments
                .Last();

            SyntaxNode result = ConcatinationUtilities.GetConcatinatedNode(argument);

            Assert.NotNull(result);
            Assert.Equal(@"""echo "" + name", result.ToString());
        }

        [Fact]
        public void IsSafeValueConcatinationReturnsFalseIfNoSafeConcatinationTypesGiven()
        {
            string code = @"
            using System;

            public class TestClass
            {
                public void testMethod(string name)
                {
                    string arguments1 = ""echo "" + name;
                    string arguments2 = arguments1;
                    System.Diagnostics.Process.Start(""CMD.exe"", arguments2);
                }
            }
            ";

            IReadOnlyDictionary<Type, IReadOnlyCollection<Type>> safeConcatinationTypes = new Dictionary<Type, IReadOnlyCollection<Type>>();

            CompilationUnitSyntax root = CSharpSyntaxTree.ParseText(code).GetCompilationUnitRoot();
            ArgumentSyntax argument = root.DescendantNodes()
                .OfType<ArgumentListSyntax>()
                .Last()
                .Arguments
                .Last();

            SyntaxNode concatination = ConcatinationUtilities.GetConcatinatedNode(argument);
            bool result = ConcatinationUtilities.IsSafeValueConcatination(concatination as BinaryExpressionSyntax, safeConcatinationTypes);

            Assert.False(result);
        }

        [Fact]
        public void IsSafeValueConcatinationReturnsFalseIfLeftSideOfConcatinationIsNotInSafeList()
        {
            string code = @"
            using System;

            public class TestClass
            {
                public void testMethod(string name)
                {
                    string arguments1 = ""echo "" + name;
                    string arguments2 = arguments1;
                    System.Diagnostics.Process.Start(""CMD.exe"", arguments2);
                }
            }
            ";

            IReadOnlyDictionary<Type, IReadOnlyCollection<Type>> safeConcatinationTypes = new Dictionary<Type, IReadOnlyCollection<Type>>
            {
                { typeof(int), new List<Type>{ typeof(int) }.AsReadOnly() }
            };

            CompilationUnitSyntax root = CSharpSyntaxTree.ParseText(code).GetCompilationUnitRoot();
            ArgumentSyntax argument = root.DescendantNodes()
                .OfType<ArgumentListSyntax>()
                .Last()
                .Arguments
                .Last();

            SyntaxNode concatination = ConcatinationUtilities.GetConcatinatedNode(argument);
            bool result = ConcatinationUtilities.IsSafeValueConcatination(concatination as BinaryExpressionSyntax, safeConcatinationTypes);

            Assert.False(result);
        }

        [Fact]
        public void IsSafeValueConcatinationReturnsTrueIfBothSidesOfConcatinationAreInSafeList()
        {
            string code = @"
            using System;

            public class TestClass
            {
                public void testMethod(string name)
                {
                    string arguments1 = ""echo "" + name;
                    string arguments2 = arguments1;
                    System.Diagnostics.Process.Start(""CMD.exe"", arguments2);
                }
            }
            ";

            IReadOnlyDictionary<Type, IReadOnlyCollection<Type>> safeConcatinationTypes = new Dictionary<Type, IReadOnlyCollection<Type>>
            {
                { typeof(string), new List<Type>{ typeof(string) }.AsReadOnly() }
            };

            CompilationUnitSyntax root = CSharpSyntaxTree.ParseText(code).GetCompilationUnitRoot();
            ArgumentSyntax argument = root.DescendantNodes()
                .OfType<ArgumentListSyntax>()
                .Last()
                .Arguments
                .Last();

            SyntaxNode concatination = ConcatinationUtilities.GetConcatinatedNode(argument);
            bool result = ConcatinationUtilities.IsSafeValueConcatination(concatination as BinaryExpressionSyntax, safeConcatinationTypes);

            Assert.True(result);
        }
    }
}
