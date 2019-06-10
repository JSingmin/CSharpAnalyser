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
    public sealed class WeakHashAnalyserTests
    {
        [Fact]
        public void WeakHashAnalyserWillNotReportVariablesNamedMD5()
        {
            string code = @"
            using System;
            using System.Security.Cryptography;

            public class TestClass
            {
                public void TestMethod()
                {
                    string MD5 = ""hash"";
                }
            }
            ";

            CompilationUnitSyntax root = CSharpSyntaxTree.ParseText(code).GetCompilationUnitRoot();

            WeakHashAnalyser analyser = new WeakHashAnalyser();
            analyser.Visit(root);
            IReadOnlyCollection<AnalyserItem> result = analyser.AnalyserItems;

            Assert.Empty(result);
        }

        [Fact]
        public void WeakHashAnalyserWillNotReportInvocationsOfMD5()
        {
            string code = @"
            using System;
            using System.Security.Cryptography;

            public class TestClass
            {
                public void TestMethod()
                {
                    var test = MD5.Create();
                }
            }
            ";

            CompilationUnitSyntax root = CSharpSyntaxTree.ParseText(code).GetCompilationUnitRoot();

            WeakHashAnalyser analyser = new WeakHashAnalyser();
            analyser.Visit(root);
            IReadOnlyCollection<AnalyserItem> result = analyser.AnalyserItems;

            Assert.NotEmpty(result);
            Assert.Equal(1, result.Count);
            Assert.Equal("Weak hash algorithm usage detected", result.First().Message);
            Assert.Equal(8, result.First().NodeReference.GetSyntax().GetLocation().GetMappedLineSpan().StartLinePosition.Line);
        }
    }
}
