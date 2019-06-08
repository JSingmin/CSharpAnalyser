using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CSharpAnalyser.Lib.Readers
{
    public static class FileReader
    {
        private const string CSharpFileExtension = "cs";

        public static async Task<ICollection<string>> ReadDirectory(string path)
        {
            if (!Directory.Exists(path)) throw new ArgumentException($"Cannot find directory {path}");

            string searchPattern = $"*.{CSharpFileExtension}";
            IList<Task<string>> fileReads = new List<Task<string>>();
            foreach(string filePath in Directory.EnumerateFiles(path, searchPattern, SearchOption.AllDirectories))
            {
                Console.WriteLine($"DEBUG: Reading {filePath}");
                fileReads.Add(File.ReadAllTextAsync(filePath));
            }

            return await Task.WhenAll(fileReads);
        }
    }
}
