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
    public sealed class SqlCommandConcatinationAnalyserTests
    {
        [Fact]
        public void SqlCommandConcatinationAnalyserReportsNothingIfArgumentsIsNotConcatenated()
        {
            string code = @"
            using System;
            using System.Data.SqlClient;

            public class TestClass
            {
                public void TestMethod(SqlConnection connection)
                {
                    SqlCommand sql = new SqlCommand(""SELECT * FROM dbo.Users"", connection);
                }
            }
            ";

            CompilationUnitSyntax root = CSharpSyntaxTree.ParseText(code).GetCompilationUnitRoot();

            SqlCommandConcatinationAnalyser analyser = new SqlCommandConcatinationAnalyser();
            analyser.Visit(root);
            IReadOnlyCollection<AnalyserItem> result = analyser.AnalyserItems;

            Assert.Empty(result);
        }

        [Fact]
        public void SqlCommandConcatinationAnalyserReportsNothingIfArgumentsVariableIsNotConcatenated()
        {
            string code = @"
            using System;
            using System.Data.SqlClient;

            public class TestClass
            {
                public void TestMethod(SqlConnection connection)
                {
                    string cmd = ""SELECT * FROM dbo.Users"";
                    SqlCommand sql = new SqlCommand(cmd, connection);
                }
            }
            ";

            CompilationUnitSyntax root = CSharpSyntaxTree.ParseText(code).GetCompilationUnitRoot();

            SqlCommandConcatinationAnalyser analyser = new SqlCommandConcatinationAnalyser();
            analyser.Visit(root);
            IReadOnlyCollection<AnalyserItem> result = analyser.AnalyserItems;

            Assert.Empty(result);
        }

        [Fact]
        public void SqlCommandConcatinationAnalyserReportsNothingIfArgumentsVariableIsParameter()
        {
            string code = @"
            using System;
            using System.Data.SqlClient;

            public class TestClass
            {
                public void TestMethod(SqlConnection connection, string cmd)
                {
                    SqlCommand sql = new SqlCommand(cmd, connection);
                }
            }
            ";

            CompilationUnitSyntax root = CSharpSyntaxTree.ParseText(code).GetCompilationUnitRoot();

            SqlCommandConcatinationAnalyser analyser = new SqlCommandConcatinationAnalyser();
            analyser.Visit(root);
            IReadOnlyCollection<AnalyserItem> result = analyser.AnalyserItems;

            Assert.Empty(result);
        }

        [Fact]
        public void SqlCommandConcatinationAnalyserReportsNothingIfArgumentsIsMethodCall()
        {
            string code = @"
            using System;
            using System.Data.SqlClient;

            public class TestClass
            {
                public void TestMethod(SqlConnection connection)
                {
                    SqlCommand sql = new SqlCommand(this.GetCommand(), connection);
                }

                private string GetCommand()
                {
                    return ""SELECT * FROM dbo.Users"";
                }
            }
            ";

            CompilationUnitSyntax root = CSharpSyntaxTree.ParseText(code).GetCompilationUnitRoot();

            SqlCommandConcatinationAnalyser analyser = new SqlCommandConcatinationAnalyser();
            analyser.Visit(root);
            IReadOnlyCollection<AnalyserItem> result = analyser.AnalyserItems;

            Assert.Empty(result);
        }

        [Fact]
        public void SqlCommandConcatinationAnalyserReportsIfArgumentsIsConcatenated()
        {
            string code = @"
            using System;
            using System.Data.SqlClient;

            public class TestClass
            {
                public void TestMethod(SqlConnection connection)
                {
                    SqlCommand sql = new SqlCommand(""SELECT * FROM dbo.Users WHERE id = "" + 1, connection);
                }
            }
            ";

            CompilationUnitSyntax root = CSharpSyntaxTree.ParseText(code).GetCompilationUnitRoot();

            SqlCommandConcatinationAnalyser analyser = new SqlCommandConcatinationAnalyser();
            analyser.Visit(root);
            IReadOnlyCollection<AnalyserItem> result = analyser.AnalyserItems;

            Assert.NotEmpty(result);
            Assert.Equal(1, result.Count);
            Assert.Equal("Concatinated SQL string", result.Last().Message);
            Assert.Equal(8, result.Last().NodeReference.GetSyntax().GetLocation().GetMappedLineSpan().StartLinePosition.Line);
        }

        [Fact]
        public void SqlCommandConcatinationAnalyserReportsIfArgumentsIsConcatenatedWithParameter()
        {
            string code = @"
            using System;
            using System.Data.SqlClient;

            public class TestClass
            {
                public void TestMethod(SqlConnection connection, int id)
                {
                    SqlCommand sql = new SqlCommand(""SELECT * FROM dbo.Users WHERE id = "" + id, connection);
                }
            }
            ";

            CompilationUnitSyntax root = CSharpSyntaxTree.ParseText(code).GetCompilationUnitRoot();

            SqlCommandConcatinationAnalyser analyser = new SqlCommandConcatinationAnalyser();
            analyser.Visit(root);
            IReadOnlyCollection<AnalyserItem> result = analyser.AnalyserItems;

            Assert.NotEmpty(result);
            Assert.Equal(1, result.Count);
            Assert.Equal("Concatinated SQL string", result.Last().Message);
            Assert.Equal(8, result.Last().NodeReference.GetSyntax().GetLocation().GetMappedLineSpan().StartLinePosition.Line);
        }

        [Fact]
        public void SqlCommandConcatinationAnalyserReportsIfArgumentsIsConcatenatedWithMethodCall()
        {
            string code = @"
            using System;
            using System.Data.SqlClient;

            public class TestClass
            {
                public void TestMethod(SqlConnection connection)
                {
                    SqlCommand sql = new SqlCommand(""SELECT * FROM dbo.Users WHERE id = "" + GetID(), connection);
                }

                private int GetID()
                {
                    return 1;
                }
            }
            ";

            CompilationUnitSyntax root = CSharpSyntaxTree.ParseText(code).GetCompilationUnitRoot();

            SqlCommandConcatinationAnalyser analyser = new SqlCommandConcatinationAnalyser();
            analyser.Visit(root);
            IReadOnlyCollection<AnalyserItem> result = analyser.AnalyserItems;

            Assert.NotEmpty(result);
            Assert.Equal(1, result.Count);
            Assert.Equal("Concatinated SQL string", result.Last().Message);
            Assert.Equal(8, result.Last().NodeReference.GetSyntax().GetLocation().GetMappedLineSpan().StartLinePosition.Line);
        }

        [Fact]
        public void SqlCommandConcatinationAnalyserReportsIfArgumentsVariableIsConcatenated()
        {
            string code = @"
            using System;
            using System.Data.SqlClient;

            public class TestClass
            {
                public void TestMethod(SqlConnection connection)
                {
                    string cmd = ""SELECT * FROM dbo.Users WHERE id = "" + 1;
                    SqlCommand sql = new SqlCommand(cmd, connection);
                }
            }
            ";

            CompilationUnitSyntax root = CSharpSyntaxTree.ParseText(code).GetCompilationUnitRoot();

            SqlCommandConcatinationAnalyser analyser = new SqlCommandConcatinationAnalyser();
            analyser.Visit(root);
            IReadOnlyCollection<AnalyserItem> result = analyser.AnalyserItems;

            Assert.NotEmpty(result);
            Assert.Equal(1, result.Count);
            Assert.Equal("Concatinated SQL string", result.Last().Message);
            Assert.Equal(9, result.Last().NodeReference.GetSyntax().GetLocation().GetMappedLineSpan().StartLinePosition.Line);
        }

        [Fact]
        public void SqlCommandConcatinationAnalyserReportsIfArgumentsVariableIsConcatenatedWithParameter()
        {
            string code = @"
            using System;
            using System.Data.SqlClient;

            public class TestClass
            {
                public void TestMethod(SqlConnection connection, int id)
                {
                    string cmd = ""SELECT * FROM dbo.Users WHERE id = "" + id;
                    SqlCommand sql = new SqlCommand(cmd, connection);
                }
            }
            ";

            CompilationUnitSyntax root = CSharpSyntaxTree.ParseText(code).GetCompilationUnitRoot();

            SqlCommandConcatinationAnalyser analyser = new SqlCommandConcatinationAnalyser();
            analyser.Visit(root);
            IReadOnlyCollection<AnalyserItem> result = analyser.AnalyserItems;

            Assert.NotEmpty(result);
            Assert.Equal(1, result.Count);
            Assert.Equal("Concatinated SQL string", result.Last().Message);
            Assert.Equal(9, result.Last().NodeReference.GetSyntax().GetLocation().GetMappedLineSpan().StartLinePosition.Line);
        }

        [Fact]
        public void SqlCommandConcatinationAnalyserReportsIfArgumentsVariableIsConcatenatedWithMethodCall()
        {
            string code = @"
            using System;
            using System.Data.SqlClient;

            public class TestClass
            {
                public void TestMethod(SqlConnection connection)
                {
                    string cmd = ""SELECT * FROM dbo.Users WHERE id = "" + this.GetID();
                    SqlCommand sql = new SqlCommand(cmd, connection);
                }

                private int GetID()
                {
                    return 1;
                }
            }
            ";

            CompilationUnitSyntax root = CSharpSyntaxTree.ParseText(code).GetCompilationUnitRoot();

            SqlCommandConcatinationAnalyser analyser = new SqlCommandConcatinationAnalyser();
            analyser.Visit(root);
            IReadOnlyCollection<AnalyserItem> result = analyser.AnalyserItems;

            Assert.NotEmpty(result);
            Assert.Equal(1, result.Count);
            Assert.Equal("Concatinated SQL string", result.Last().Message);
            Assert.Equal(9, result.Last().NodeReference.GetSyntax().GetLocation().GetMappedLineSpan().StartLinePosition.Line);
        }
    }
}
