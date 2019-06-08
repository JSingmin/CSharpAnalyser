using System;
using System.IO;
using System.Threading.Tasks;
using CSharpAnalyser.Lib.Readers;
using CSharpAnalyser.Lib.Services;

namespace CSharpAnalyser.App
{
    class Program
    {
        static async Task Main(string[] args)
        {
            //if (args.Length == 0) throw new ArgumentException("Directory required as argument");
            var directory = "ExampleCode";

            try
            {
                //AnalyserService.DoStuff2(args[0].Trim());
                await AnalyserService.DoStuff2(directory);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error analysing files: {e.Message}");
                Console.WriteLine(e.StackTrace);
            }
        }
    }
}
