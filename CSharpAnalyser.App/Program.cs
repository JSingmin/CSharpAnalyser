using System;
using System.IO;
using System.Threading.Tasks;
using CSharpAnalyser.Lib.Services;

namespace CSharpAnalyser.App
{
    class Program
    {
        static async Task Main(string[] args)
        {
            if (args.Length == 0) throw new ArgumentException("Directory required as argument");

            try
            {
                await AnalyserService.AnalyseCodeInDirectory(args[0].Trim());
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error analysing files: {e.Message}");
                Console.WriteLine(e.StackTrace);
            }
        }
    }
}
