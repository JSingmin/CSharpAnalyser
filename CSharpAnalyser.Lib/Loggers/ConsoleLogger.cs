using System;

namespace CSharpAnalyser.Lib.Loggers
{
    public static class ConsoleLogger
    {
        public static void Error(string message)
        {
            Console.WriteLine($"ERROR: {message}");
        }
    }
}
