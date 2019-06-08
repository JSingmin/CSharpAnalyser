using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp;
using CSharpAnalyser.Lib.Analysers;
using CSharpAnalyser.Lib.Models;
using CSharpAnalyser.Lib.Parsers;
using CSharpAnalyser.Lib.Reporters;

namespace CSharpAnalyser.Lib.Services
{
    public class AnalyserService
    {
        private static IReadOnlyCollection<CSharpSyntaxWalker> Analysers => new List<CSharpSyntaxWalker>
        {
            new ProcessUnsafeConcatinationAnalyser(),
            new SqlCommandConcatinationAnalyser(),
            new SqlCommandUnsafeConcatinationAnalyser(),
            new WeakHashAnalyser()
        }.AsReadOnly();

        public static void DoStuff(ICollection<string> files)
        {
            if (files == null) throw new ArgumentNullException(nameof(files));

            var something = files.Select(f => StringParser.Parse(f));
        }

        public static async Task DoStuff2(string filePath)
        {
            Console.WriteLine($"DEBUG: {nameof(AnalyserService)}.{nameof(DoStuff2)} begin");
            if (string.IsNullOrWhiteSpace(filePath)) throw new ArgumentNullException(nameof(filePath));

            var project = DirectoryParser.Parse(filePath);
            var syntaxTrees = await Task.WhenAll(project.Documents.Select(d => d.GetSyntaxTreeAsync()));
            var compilationRoots = syntaxTrees.Select(t => t.GetCompilationUnitRoot());
            var test = new DebugAnalyser();
            foreach(var compilationRoot in compilationRoots)
            {
                //test.Visit(compilationRoot);
            }

            List<AnalyserItem> analyserItems = new List<AnalyserItem>();
            foreach (var analyser in Analysers)
            {
                Console.WriteLine($"DEBUG: Starting {analyser.GetType().Name}");
                foreach(var compilationRoot in compilationRoots)
                {
                    analyser.Visit(compilationRoot);
                }
                Console.WriteLine($"DEBUG: Finished {analyser.GetType().Name}");
                analyserItems.AddRange((analyser as IAnalyser).AnalyserItems);
            }

            await ConsoleReporter.Report(analyserItems);
            Console.WriteLine($"DEBUG: {nameof(AnalyserService)}.{nameof(DoStuff2)} end");
        }
    }
}
