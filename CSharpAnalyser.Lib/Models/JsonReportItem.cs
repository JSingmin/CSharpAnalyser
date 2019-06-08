using System;
using Newtonsoft.Json;

namespace CSharpAnalyser.Lib.Models
{
    public class JsonReportItem
    {
        public string Message { get; set; }

        public string FileName { get; set; }

        public int LineNumber { get; set; }
    }
}
