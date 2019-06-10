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
    public sealed class SqlCommandUnsafeConcatinationAnalyserTests
    {
        [Fact]
        public void SqlCommandUnsafeConcatinationAnalyserReportsNothingIfArgumentsIsNotConcatenated()
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

            SqlCommandUnsafeConcatinationAnalyser analyser = new SqlCommandUnsafeConcatinationAnalyser();
            analyser.Visit(root);
            IReadOnlyCollection<AnalyserItem> result = analyser.AnalyserItems;

            Assert.Empty(result);
        }

        [Fact]
        public void SqlCommandUnsafeConcatinationAnalyserReportsNothingIfArgumentsVariableIsNotConcatenated()
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

            SqlCommandUnsafeConcatinationAnalyser analyser = new SqlCommandUnsafeConcatinationAnalyser();
            analyser.Visit(root);
            IReadOnlyCollection<AnalyserItem> result = analyser.AnalyserItems;

            Assert.Empty(result);
        }

        [Fact]
        public void SqlCommandUnsafeConcatinationAnalyserReportsNothingIfArgumentsVariableIsParameter()
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

            SqlCommandUnsafeConcatinationAnalyser analyser = new SqlCommandUnsafeConcatinationAnalyser();
            analyser.Visit(root);
            IReadOnlyCollection<AnalyserItem> result = analyser.AnalyserItems;

            Assert.Empty(result);
        }

        [Fact]
        public void SqlCommandUnsafeConcatinationAnalyserReportsNothingIfArgumentsIsMethodCall()
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

            SqlCommandUnsafeConcatinationAnalyser analyser = new SqlCommandUnsafeConcatinationAnalyser();
            analyser.Visit(root);
            IReadOnlyCollection<AnalyserItem> result = analyser.AnalyserItems;

            Assert.Empty(result);
        }

        [Fact]
        public void SqlCommandUnsafeConcatinationAnalyserReportsNothingIfArgumentsIsSafelyConcatenated()
        {
            string code = @"
            using System;
            using System.Data.SqlClient;

            public class TestClass
            {
                public void TestMethod(SqlConnection connection)
                {
                    SqlCommand sql = new SqlCommand(""SELECT * FROM dbo.Users WHERE id = "" + 3.14, connection);
                }
            }
            ";

            CompilationUnitSyntax root = CSharpSyntaxTree.ParseText(code).GetCompilationUnitRoot();

            SqlCommandUnsafeConcatinationAnalyser analyser = new SqlCommandUnsafeConcatinationAnalyser();
            analyser.Visit(root);
            IReadOnlyCollection<AnalyserItem> result = analyser.AnalyserItems;

            Assert.Empty(result);
        }

        [Fact]
        public void SqlCommandUnsafeConcatinationAnalyserReportsNothingIfArgumentsIsSafelyConcatenatedWithParameter()
        {
            string code = @"
            using System;
            using System.Data.SqlClient;

            public class TestClass
            {
                public void TestMethod(SqlConnection connection, decimal id)
                {
                    SqlCommand sql = new SqlCommand(""SELECT * FROM dbo.Users WHERE id = "" + id, connection);
                }
            }
            ";

            CompilationUnitSyntax root = CSharpSyntaxTree.ParseText(code).GetCompilationUnitRoot();

            SqlCommandUnsafeConcatinationAnalyser analyser = new SqlCommandUnsafeConcatinationAnalyser();
            analyser.Visit(root);
            IReadOnlyCollection<AnalyserItem> result = analyser.AnalyserItems;

            Assert.Empty(result);
        }

        [Fact]
        public void SqlCommandUnsafeConcatinationAnalyserReportsNothingIfArgumentsIsSafelyConcatenatedWithMethodCall()
        {
            string code = @"
            using System;
            using System.Data.SqlClient;

            public class TestClass
            {
                public void TestMethod(SqlConnection connection)
                {
                    SqlCommand sql = new SqlCommand(""SELECT * FROM dbo.Users WHERE id = "" + this.GetID(), connection);
                }

                private decimal GetID()
                {
                    return 3.14;
                }
            }
            ";

            CompilationUnitSyntax root = CSharpSyntaxTree.ParseText(code).GetCompilationUnitRoot();

            SqlCommandUnsafeConcatinationAnalyser analyser = new SqlCommandUnsafeConcatinationAnalyser();
            analyser.Visit(root);
            IReadOnlyCollection<AnalyserItem> result = analyser.AnalyserItems;

            Assert.Empty(result);
        }

        [Fact]
        public void SqlCommandUnsafeConcatinationAnalyserReportsNothingIfArgumentsVariableIsSafelyConcatenated()
        {
            string code = @"
            using System;
            using System.Data.SqlClient;

            public class TestClass
            {
                public void TestMethod(SqlConnection connection)
                {
                    string cmd = ""SELECT * FROM dbo.Users WHERE id = "" + 2;
                    SqlCommand sql = new SqlCommand(cmd, connection);
                }
            }
            ";

            CompilationUnitSyntax root = CSharpSyntaxTree.ParseText(code).GetCompilationUnitRoot();

            SqlCommandUnsafeConcatinationAnalyser analyser = new SqlCommandUnsafeConcatinationAnalyser();
            analyser.Visit(root);
            IReadOnlyCollection<AnalyserItem> result = analyser.AnalyserItems;

            Assert.Empty(result);
        }

        [Fact]
        public void SqlCommandUnsafeConcatinationAnalyserReportsNothingIfArgumentsVariableIsSafelyConcatenatedWithParameter()
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

            SqlCommandUnsafeConcatinationAnalyser analyser = new SqlCommandUnsafeConcatinationAnalyser();
            analyser.Visit(root);
            IReadOnlyCollection<AnalyserItem> result = analyser.AnalyserItems;

            Assert.Empty(result);
        }

        [Fact]
        public void SqlCommandUnsafeConcatinationAnalyserReportsNothingIfArgumentsVariableIsSafelyConcatenatedWithMethodCall()
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
                    return 2;
                }
            }
            ";

            CompilationUnitSyntax root = CSharpSyntaxTree.ParseText(code).GetCompilationUnitRoot();

            SqlCommandUnsafeConcatinationAnalyser analyser = new SqlCommandUnsafeConcatinationAnalyser();
            analyser.Visit(root);
            IReadOnlyCollection<AnalyserItem> result = analyser.AnalyserItems;

            Assert.Empty(result);
        }

        [Fact]
        public void SqlCommandUnsafeConcatinationAnalyserReportsIfArgumentsIsNotSafelyConcatenated()
        {
            string code = @"
            using System;
            using System.Data.SqlClient;

            public class TestClass
            {
                public void TestMethod(SqlConnection connection)
                {
                    SqlCommand sql = new SqlCommand(""SELECT * FROM dbo.Users"" + "" WHERE id = 1"", connection);
                }
            }
            ";

            CompilationUnitSyntax root = CSharpSyntaxTree.ParseText(code).GetCompilationUnitRoot();

            SqlCommandUnsafeConcatinationAnalyser analyser = new SqlCommandUnsafeConcatinationAnalyser();
            analyser.Visit(root);
            IReadOnlyCollection<AnalyserItem> result = analyser.AnalyserItems;

            Assert.NotEmpty(result);
            Assert.Equal(1, result.Count);
            Assert.Equal("Concatinated SQL string is unsafe", result.Last().Message);
            Assert.Equal(8, result.Last().NodeReference.GetSyntax().GetLocation().GetMappedLineSpan().StartLinePosition.Line);
        }

        [Fact]
        public void SqlCommandUnsafeConcatinationAnalyserReportsIfArgumentsIsNotSafelyConcatenatedWithParameter()
        {
            string code = @"
            using System;
            using System.Data.SqlClient;

            public class TestClass
            {
                public void TestMethod(SqlConnection connection, string conditions)
                {
                    SqlCommand sql = new SqlCommand(""SELECT * FROM dbo.Users"" + conditions, connection);
                }
            }
            ";

            CompilationUnitSyntax root = CSharpSyntaxTree.ParseText(code).GetCompilationUnitRoot();

            SqlCommandUnsafeConcatinationAnalyser analyser = new SqlCommandUnsafeConcatinationAnalyser();
            analyser.Visit(root);
            IReadOnlyCollection<AnalyserItem> result = analyser.AnalyserItems;

            Assert.NotEmpty(result);
            Assert.Equal(1, result.Count);
            Assert.Equal("Concatinated SQL string is unsafe", result.Last().Message);
            Assert.Equal(8, result.Last().NodeReference.GetSyntax().GetLocation().GetMappedLineSpan().StartLinePosition.Line);
        }

        [Fact]
        public void SqlCommandUnsafeConcatinationAnalyserReportsIfArgumentsIsNotSafelyConcatenatedWithMethodCall()
        {
            string code = @"
            using System;
            using System.Data.SqlClient;

            public class TestClass
            {
                public void TestMethod(SqlConnection connection)
                {
                    SqlCommand sql = new SqlCommand(""SELECT * FROM dbo.Users"" + GetConditions(), connection);
                }

                private string GetConditions()
                {
                    return "" WHERE id = 1"";
                }
            }
            ";

            CompilationUnitSyntax root = CSharpSyntaxTree.ParseText(code).GetCompilationUnitRoot();

            SqlCommandUnsafeConcatinationAnalyser analyser = new SqlCommandUnsafeConcatinationAnalyser();
            analyser.Visit(root);
            IReadOnlyCollection<AnalyserItem> result = analyser.AnalyserItems;

            Assert.NotEmpty(result);
            Assert.Equal(1, result.Count);
            Assert.Equal("Concatinated SQL string is unsafe", result.Last().Message);
            Assert.Equal(8, result.Last().NodeReference.GetSyntax().GetLocation().GetMappedLineSpan().StartLinePosition.Line);
        }

        [Fact]
        public void SqlCommandUnsafeConcatinationAnalyserReportsIfArgumentsVariableIsNotSafelyConcatenated()
        {
            string code = @"
            using System;
            using System.Data.SqlClient;

            public class TestClass
            {
                public void TestMethod(SqlConnection connection)
                {
                    string cmd = ""SELECT * FROM dbo.Users"" + "" WHERE id = 1"";
                    SqlCommand sql = new SqlCommand(cmd, connection);
                }
            }
            ";

            CompilationUnitSyntax root = CSharpSyntaxTree.ParseText(code).GetCompilationUnitRoot();

            SqlCommandUnsafeConcatinationAnalyser analyser = new SqlCommandUnsafeConcatinationAnalyser();
            analyser.Visit(root);
            IReadOnlyCollection<AnalyserItem> result = analyser.AnalyserItems;

            Assert.NotEmpty(result);
            Assert.Equal(1, result.Count);
            Assert.Equal("Concatinated SQL string is unsafe", result.Last().Message);
            Assert.Equal(9, result.Last().NodeReference.GetSyntax().GetLocation().GetMappedLineSpan().StartLinePosition.Line);
        }

        [Fact]
        public void SqlCommandUnsafeConcatinationAnalyserReportsIfArgumentsVariableIsNotSafelyConcatenatedWithParameter()
        {
            string code = @"
            using System;
            using System.Data.SqlClient;

            public class TestClass
            {
                public void TestMethod(SqlConnection connection, string conditions)
                {
                    string cmd = ""SELECT * FROM dbo.Users"" + conditions;
                    SqlCommand sql = new SqlCommand(cmd, connection);
                }
            }
            ";

            CompilationUnitSyntax root = CSharpSyntaxTree.ParseText(code).GetCompilationUnitRoot();

            SqlCommandUnsafeConcatinationAnalyser analyser = new SqlCommandUnsafeConcatinationAnalyser();
            analyser.Visit(root);
            IReadOnlyCollection<AnalyserItem> result = analyser.AnalyserItems;

            Assert.NotEmpty(result);
            Assert.Equal(1, result.Count);
            Assert.Equal("Concatinated SQL string is unsafe", result.Last().Message);
            Assert.Equal(9, result.Last().NodeReference.GetSyntax().GetLocation().GetMappedLineSpan().StartLinePosition.Line);
        }

        [Fact]
        public void SqlCommandUnsafeConcatinationAnalyserReportsIfArgumentsVariableIsNotSafelyConcatenatedWithMethodCall()
        {
            string code = @"
            using System;
            using System.Data.SqlClient;

            public class TestClass
            {
                public void TestMethod(SqlConnection connection)
                {
                    string cmd = ""SELECT * FROM dbo.Users"" + this.GetConditions();
                    SqlCommand sql = new SqlCommand(cmd, connection);
                }

                private string GetConditions()
                {
                    return "" WHERE id = 1"";
                }
            }
            ";

            CompilationUnitSyntax root = CSharpSyntaxTree.ParseText(code).GetCompilationUnitRoot();

            SqlCommandUnsafeConcatinationAnalyser analyser = new SqlCommandUnsafeConcatinationAnalyser();
            analyser.Visit(root);
            IReadOnlyCollection<AnalyserItem> result = analyser.AnalyserItems;

            Assert.NotEmpty(result);
            Assert.Equal(1, result.Count);
            Assert.Equal("Concatinated SQL string is unsafe", result.Last().Message);
            Assert.Equal(9, result.Last().NodeReference.GetSyntax().GetLocation().GetMappedLineSpan().StartLinePosition.Line);
        }
    }
}
