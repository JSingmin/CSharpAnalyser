using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Xunit;
using CSharpAnalyser.Lib.Analysers;
using CSharpAnalyser.Lib.Models;

namespace CSharpAnalyser.Lib.Tests.Analysers
{
    public sealed class UnusedMethodAnalyserTests
    {
        [Fact]
        public void UnusedMethodAnalyserReportsUnusedMethods()
        {
            string code = @"
            using System;

            public class TestClass
            {
                public void TestMethod()
                {
                    this.TestMethod2();
                }

                private void TestMethod2()
                {
                    var result = 1 + 1;
                }
            }
            ";

            CompilationUnitSyntax root = CSharpSyntaxTree.ParseText(code).GetCompilationUnitRoot();

            UnusedMethodAnalyser analyser = new UnusedMethodAnalyser();
            analyser.Visit(root);
            IReadOnlyCollection<AnalyserItem> result = analyser.AnalyserItems;

            Assert.NotEmpty(result);
            Assert.Equal(1, result.Count);
            Assert.Equal("Unused method", result.First().Message);
            Assert.Equal(5, result.First().NodeReference.GetSyntax().GetLocation().GetMappedLineSpan().StartLinePosition.Line);
        }

        [Fact]
        public void UnusedMethodAnalyserReportsUnusedOverloadedMethods()
        {
            string code = @"
            using System;

            public class TestClass
            {
                public void TestMethod()
                {
                    this.TestMethod2();
                }

                private void TestMethod2()
                {
                    var result = 1 + 1;
                }

                private void TestMethod2(int i)
                {
                    var result = 1 + i;
                }
            }
            ";

            CompilationUnitSyntax root = CSharpSyntaxTree.ParseText(code).GetCompilationUnitRoot();

            UnusedMethodAnalyser analyser = new UnusedMethodAnalyser();
            analyser.Visit(root);
            IReadOnlyCollection<AnalyserItem> result = analyser.AnalyserItems;

            Assert.NotEmpty(result);
            Assert.Equal(2, result.Count);
            Assert.Equal("Unused method", result.First().Message);
            Assert.Equal(5, result.First().NodeReference.GetSyntax().GetLocation().GetMappedLineSpan().StartLinePosition.Line);
            Assert.Equal("Unused method", result.Last().Message);
            Assert.Equal(15, result.Last().NodeReference.GetSyntax().GetLocation().GetMappedLineSpan().StartLinePosition.Line);
        }

        [Fact]
        public void UnusedMethodAnalyserReportsNothingIfAllMethodsAreUsed()
        {
            string code = @"
            using System;

            public class TestClass1
            {
                public void TestMethod()
                {
                    TestClass2 test = new TestClass2();
                    test.TestMethod();
                }
            }

            public class TestClass2
            {
                public void TestMethod()
                {
                    TestClass1 test = new TestClass1();
                    test.TestMethod();
                }
            }
            ";

            CompilationUnitSyntax root = CSharpSyntaxTree.ParseText(code).GetCompilationUnitRoot();

            UnusedMethodAnalyser analyser = new UnusedMethodAnalyser();
            analyser.Visit(root);
            IReadOnlyCollection<AnalyserItem> result = analyser.AnalyserItems;

            Assert.Empty(result);
        }
    }
}
