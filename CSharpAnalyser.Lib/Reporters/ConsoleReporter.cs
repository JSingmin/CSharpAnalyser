using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Newtonsoft.Json;
using CSharpAnalyser.Lib.Models;

namespace CSharpAnalyser.Lib.Reporters
{
    public static class ConsoleReporter
    {
        public static async Task Report(IReadOnlyCollection<AnalyserItem> analyserItems)
        {
            string jsonString = await ConvertToJsonString(analyserItems);
            Console.WriteLine(jsonString);
        }

        private static async Task<string> ConvertToJsonString(IReadOnlyCollection<AnalyserItem> analyserItems)
        {
            var reportItems = await Task.WhenAll(analyserItems.Select(i => ConvertToJsonReportItem(i)));
            return JsonConvert.SerializeObject(reportItems, Formatting.Indented);
        }

        private static async Task<JsonReportItem> ConvertToJsonReportItem(AnalyserItem analyserItem)
        {
            SyntaxNode node = await analyserItem.NodeReference.GetSyntaxAsync();
            FileLinePositionSpan nodeLocation = node.GetLocation().GetMappedLineSpan();
            return new JsonReportItem
            {
                Message = analyserItem.Message,
                FileName = nodeLocation.Path,
                // Line numbers are zero-indexed
                LineNumber = nodeLocation.StartLinePosition.Line + 1
            };
        }
    }
}
