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
            // Commenting this analyser out, as it was a stepping-stone task of the exercise
            // new SqlCommandConcatinationAnalyser(),
            new SqlCommandUnsafeConcatinationAnalyser(),
            new SystemProcessUnsafeConcatinationAnalyser(),
            new WeakHashAnalyser(),
            new UnusedMethodAnalyser()
        }.AsReadOnly();

        public static async Task AnalyseCodeInDirectory(string directoryName)
        {
            Console.WriteLine($"DEBUG: {nameof(AnalyserService)}.{nameof(AnalyseCodeInDirectory)} begin");
            if (string.IsNullOrWhiteSpace(directoryName)) throw new ArgumentNullException(nameof(directoryName));

            var project = DirectoryParser.Parse(directoryName);
            var syntaxTrees = await Task.WhenAll(project.Documents.Select(d => d.GetSyntaxTreeAsync()));
            var compilationRoots = syntaxTrees.Select(t => t.GetCompilationUnitRoot());

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
            Console.WriteLine($"DEBUG: {nameof(AnalyserService)}.{nameof(AnalyseCodeInDirectory)} end");
        }
    }
}
