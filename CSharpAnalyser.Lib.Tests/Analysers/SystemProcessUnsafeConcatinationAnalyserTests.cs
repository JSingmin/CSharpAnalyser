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
    public sealed class SystemProcessUnsafeConcatinationAnalyserTests
    {
        [Fact]
        public void SystemProcessUnsafeConcatinationAnalyserReportsNothingIfArgumentsIsNotConcatenated()
        {
            string code = @"
            using System;

            public class TestClass
            {
                public void TestMethod()
                {
                    System.Diagnostics.Process.Start(""CMD.exe"", ""echo Hello"");
                }
            }
            ";

            CompilationUnitSyntax root = CSharpSyntaxTree.ParseText(code).GetCompilationUnitRoot();

            SystemProcessUnsafeConcatinationAnalyser analyser = new SystemProcessUnsafeConcatinationAnalyser();
            analyser.Visit(root);
            IReadOnlyCollection<AnalyserItem> result = analyser.AnalyserItems;

            Assert.Empty(result);
        }

        [Fact]
        public void SystemProcessUnsafeConcatinationAnalyserReportsNothingIfArgumentsVariableIsNotConcatenated()
        {
            string code = @"
            using System;

            public class TestClass
            {
                public void TestMethod()
                {
                    string arguments = ""echo Hello"";
                    System.Diagnostics.Process.Start(""CMD.exe"", arguments);
                }
            }
            ";

            CompilationUnitSyntax root = CSharpSyntaxTree.ParseText(code).GetCompilationUnitRoot();

            SystemProcessUnsafeConcatinationAnalyser analyser = new SystemProcessUnsafeConcatinationAnalyser();
            analyser.Visit(root);
            IReadOnlyCollection<AnalyserItem> result = analyser.AnalyserItems;

            Assert.Empty(result);
        }

        [Fact]
        public void SystemProcessUnsafeConcatinationAnalyserReportsNothingIfArgumentsVariableIsParameter()
        {
            string code = @"
            using System;

            public class TestClass
            {
                public void TestMethod(string arguments)
                {
                    System.Diagnostics.Process.Start(""CMD.exe"", arguments);
                }
            }
            ";

            CompilationUnitSyntax root = CSharpSyntaxTree.ParseText(code).GetCompilationUnitRoot();

            SystemProcessUnsafeConcatinationAnalyser analyser = new SystemProcessUnsafeConcatinationAnalyser();
            analyser.Visit(root);
            IReadOnlyCollection<AnalyserItem> result = analyser.AnalyserItems;

            Assert.Empty(result);
        }

        [Fact]
        public void SystemProcessUnsafeConcatinationAnalyserReportsNothingIfArgumentsIsMethodCall()
        {
            string code = @"
            using System;

            public class TestClass
            {
                public void TestMethod()
                {
                    System.Diagnostics.Process.Start(""CMD.exe"", this.GetArguments());
                }

                private string GetArguments()
                {
                    return ""echo hello"";
                }
            }
            ";

            CompilationUnitSyntax root = CSharpSyntaxTree.ParseText(code).GetCompilationUnitRoot();

            SystemProcessUnsafeConcatinationAnalyser analyser = new SystemProcessUnsafeConcatinationAnalyser();
            analyser.Visit(root);
            IReadOnlyCollection<AnalyserItem> result = analyser.AnalyserItems;

            Assert.Empty(result);
        }

        [Fact]
        public void SystemProcessUnsafeConcatinationAnalyserReportsNothingIfArgumentsIsSafelyConcatenated()
        {
            string code = @"
            using System;

            public class TestClass
            {
                public void TestMethod()
                {
                    System.Diagnostics.Process.Start(""CMD.exe"", ""echo Hello "" + 3.14);
                }
            }
            ";

            CompilationUnitSyntax root = CSharpSyntaxTree.ParseText(code).GetCompilationUnitRoot();

            SystemProcessUnsafeConcatinationAnalyser analyser = new SystemProcessUnsafeConcatinationAnalyser();
            analyser.Visit(root);
            IReadOnlyCollection<AnalyserItem> result = analyser.AnalyserItems;

            Assert.Empty(result);
        }

        [Fact]
        public void SystemProcessUnsafeConcatinationAnalyserReportsNothingIfArgumentsIsSafelyConcatenatedWithParameter()
        {
            string code = @"
            using System;

            public class TestClass
            {
                public void TestMethod(decimal number)
                {
                    System.Diagnostics.Process.Start(""CMD.exe"", ""echo Hello "" + number);
                }
            }
            ";

            CompilationUnitSyntax root = CSharpSyntaxTree.ParseText(code).GetCompilationUnitRoot();

            SystemProcessUnsafeConcatinationAnalyser analyser = new SystemProcessUnsafeConcatinationAnalyser();
            analyser.Visit(root);
            IReadOnlyCollection<AnalyserItem> result = analyser.AnalyserItems;

            Assert.Empty(result);
        }

        [Fact]
        public void SystemProcessUnsafeConcatinationAnalyserReportsNothingIfArgumentsIsSafelyConcatenatedWithMethodCall()
        {
            string code = @"
            using System;

            public class TestClass
            {
                public void TestMethod()
                {
                    System.Diagnostics.Process.Start(""CMD.exe"", ""echo Hello "" + this.GetNumber());
                }

                private decimal GetNumber()
                {
                    return 3.14;
                }
            }
            ";

            CompilationUnitSyntax root = CSharpSyntaxTree.ParseText(code).GetCompilationUnitRoot();

            SystemProcessUnsafeConcatinationAnalyser analyser = new SystemProcessUnsafeConcatinationAnalyser();
            analyser.Visit(root);
            IReadOnlyCollection<AnalyserItem> result = analyser.AnalyserItems;

            Assert.Empty(result);
        }

        [Fact]
        public void SystemProcessUnsafeConcatinationAnalyserReportsNothingIfArgumentsVariableIsSafelyConcatenated()
        {
            string code = @"
            using System;

            public class TestClass
            {
                public void TestMethod()
                {
                    string arguments = ""echo Hello "" + 2;
                    System.Diagnostics.Process.Start(""CMD.exe"", arguments);
                }
            }
            ";

            CompilationUnitSyntax root = CSharpSyntaxTree.ParseText(code).GetCompilationUnitRoot();

            SystemProcessUnsafeConcatinationAnalyser analyser = new SystemProcessUnsafeConcatinationAnalyser();
            analyser.Visit(root);
            IReadOnlyCollection<AnalyserItem> result = analyser.AnalyserItems;

            Assert.Empty(result);
        }

        [Fact]
        public void SystemProcessUnsafeConcatinationAnalyserReportsNothingIfArgumentsVariableIsSafelyConcatenatedWithParameter()
        {
            string code = @"
            using System;

            public class TestClass
            {
                public void TestMethod(int number)
                {
                    string arguments = ""echo Hello "" + number;
                    System.Diagnostics.Process.Start(""CMD.exe"", arguments);
                }
            }
            ";

            CompilationUnitSyntax root = CSharpSyntaxTree.ParseText(code).GetCompilationUnitRoot();

            SystemProcessUnsafeConcatinationAnalyser analyser = new SystemProcessUnsafeConcatinationAnalyser();
            analyser.Visit(root);
            IReadOnlyCollection<AnalyserItem> result = analyser.AnalyserItems;

            Assert.Empty(result);
        }

        [Fact]
        public void SystemProcessUnsafeConcatinationAnalyserReportsNothingIfArgumentsVariableIsSafelyConcatenatedWithMethodCall()
        {
            string code = @"
            using System;

            public class TestClass
            {
                public void TestMethod()
                {
                    string arguments = ""echo Hello "" + this.GetNumber();
                    System.Diagnostics.Process.Start(""CMD.exe"", arguments);
                }

                private int GetNumber()
                {
                    return 2;
                }
            }
            ";

            CompilationUnitSyntax root = CSharpSyntaxTree.ParseText(code).GetCompilationUnitRoot();

            SystemProcessUnsafeConcatinationAnalyser analyser = new SystemProcessUnsafeConcatinationAnalyser();
            analyser.Visit(root);
            IReadOnlyCollection<AnalyserItem> result = analyser.AnalyserItems;

            Assert.Empty(result);
        }

        [Fact]
        public void SystemProcessUnsafeConcatinationAnalyserReportsIfArgumentsIsNotSafelyConcatenated()
        {
            string code = @"
            using System;

            public class TestClass
            {
                public void TestMethod()
                {
                    System.Diagnostics.Process.Start(""CMD.exe"", ""echo Hello"" + "" World"");
                }
            }
            ";

            CompilationUnitSyntax root = CSharpSyntaxTree.ParseText(code).GetCompilationUnitRoot();

            SystemProcessUnsafeConcatinationAnalyser analyser = new SystemProcessUnsafeConcatinationAnalyser();
            analyser.Visit(root);
            IReadOnlyCollection<AnalyserItem> result = analyser.AnalyserItems;

            Assert.NotEmpty(result);
            Assert.Equal(1, result.Count);
            Assert.Equal("Concatinated process start arguments are unsafe", result.Last().Message);
            Assert.Equal(7, result.Last().NodeReference.GetSyntax().GetLocation().GetMappedLineSpan().StartLinePosition.Line);
        }

        [Fact]
        public void SystemProcessUnsafeConcatinationAnalyserReportsIfArgumentsIsNotSafelyConcatenatedWithParameter()
        {
            string code = @"
            using System;

            public class TestClass
            {
                public void TestMethod(string value)
                {
                    System.Diagnostics.Process.Start(""CMD.exe"", ""echo Hello"" + value);
                }
            }
            ";

            CompilationUnitSyntax root = CSharpSyntaxTree.ParseText(code).GetCompilationUnitRoot();

            SystemProcessUnsafeConcatinationAnalyser analyser = new SystemProcessUnsafeConcatinationAnalyser();
            analyser.Visit(root);
            IReadOnlyCollection<AnalyserItem> result = analyser.AnalyserItems;

            Assert.NotEmpty(result);
            Assert.Equal(1, result.Count);
            Assert.Equal("Concatinated process start arguments are unsafe", result.Last().Message);
            Assert.Equal(7, result.Last().NodeReference.GetSyntax().GetLocation().GetMappedLineSpan().StartLinePosition.Line);
        }

        [Fact]
        public void SystemProcessUnsafeConcatinationAnalyserReportsIfArgumentsIsNotSafelyConcatenatedWithMethodCall()
        {
            string code = @"
            using System;

            public class TestClass
            {
                public void TestMethod()
                {
                    System.Diagnostics.Process.Start(""CMD.exe"", ""echo Hello"" + this.GetString());
                }

                private string GetString()
                {
                    return "" World"";
                }
            }
            ";

            CompilationUnitSyntax root = CSharpSyntaxTree.ParseText(code).GetCompilationUnitRoot();

            SystemProcessUnsafeConcatinationAnalyser analyser = new SystemProcessUnsafeConcatinationAnalyser();
            analyser.Visit(root);
            IReadOnlyCollection<AnalyserItem> result = analyser.AnalyserItems;

            Assert.NotEmpty(result);
            Assert.Equal(1, result.Count);
            Assert.Equal("Concatinated process start arguments are unsafe", result.Last().Message);
            Assert.Equal(7, result.Last().NodeReference.GetSyntax().GetLocation().GetMappedLineSpan().StartLinePosition.Line);
        }

        [Fact]
        public void SystemProcessUnsafeConcatinationAnalyserReportsIfArgumentsVariableIsNotSafelyConcatenated()
        {
            string code = @"
            using System;

            public class TestClass
            {
                public void TestMethod()
                {
                    string arguments = ""echo Hello"" + "" World"";
                    System.Diagnostics.Process.Start(""CMD.exe"", arguments);
                }
            }
            ";

            CompilationUnitSyntax root = CSharpSyntaxTree.ParseText(code).GetCompilationUnitRoot();

            SystemProcessUnsafeConcatinationAnalyser analyser = new SystemProcessUnsafeConcatinationAnalyser();
            analyser.Visit(root);
            IReadOnlyCollection<AnalyserItem> result = analyser.AnalyserItems;

            Assert.NotEmpty(result);
            Assert.Equal(1, result.Count);
            Assert.Equal("Concatinated process start arguments are unsafe", result.Last().Message);
            Assert.Equal(8, result.Last().NodeReference.GetSyntax().GetLocation().GetMappedLineSpan().StartLinePosition.Line);
        }

        [Fact]
        public void SystemProcessUnsafeConcatinationAnalyserReportsIfArgumentsVariableIsNotSafelyConcatenatedWithParameter()
        {
            string code = @"
            using System;

            public class TestClass
            {
                public void TestMethod(string value)
                {
                    string arguments = ""echo Hello"" + value;
                    System.Diagnostics.Process.Start(""CMD.exe"", arguments);
                }
            }
            ";

            CompilationUnitSyntax root = CSharpSyntaxTree.ParseText(code).GetCompilationUnitRoot();

            SystemProcessUnsafeConcatinationAnalyser analyser = new SystemProcessUnsafeConcatinationAnalyser();
            analyser.Visit(root);
            IReadOnlyCollection<AnalyserItem> result = analyser.AnalyserItems;

            Assert.NotEmpty(result);
            Assert.Equal(1, result.Count);
            Assert.Equal("Concatinated process start arguments are unsafe", result.Last().Message);
            Assert.Equal(8, result.Last().NodeReference.GetSyntax().GetLocation().GetMappedLineSpan().StartLinePosition.Line);
        }

        [Fact]
        public void SystemProcessUnsafeConcatinationAnalyserReportsIfArgumentsVariableIsNotSafelyConcatenatedWithMethodCall()
        {
            string code = @"
            using System;

            public class TestClass
            {
                public void TestMethod()
                {
                    string arguments = ""echo Hello"" + this.GetString();
                    System.Diagnostics.Process.Start(""CMD.exe"", arguments);
                }

                private string GetString()
                {
                    return "" World"";
                }
            }
            ";

            CompilationUnitSyntax root = CSharpSyntaxTree.ParseText(code).GetCompilationUnitRoot();

            SystemProcessUnsafeConcatinationAnalyser analyser = new SystemProcessUnsafeConcatinationAnalyser();
            analyser.Visit(root);
            IReadOnlyCollection<AnalyserItem> result = analyser.AnalyserItems;

            Assert.NotEmpty(result);
            Assert.Equal(1, result.Count);
            Assert.Equal("Concatinated process start arguments are unsafe", result.Last().Message);
            Assert.Equal(8, result.Last().NodeReference.GetSyntax().GetLocation().GetMappedLineSpan().StartLinePosition.Line);
        }
    }
}
