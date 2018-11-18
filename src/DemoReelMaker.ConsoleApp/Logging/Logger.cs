using System;

namespace DemoReelMaker.ConsoleApp.Logging
{
    public static class Logger
    {
        public static void Log(string message)
        {
            Console.WriteLine($"[DemoReel] {message}");
        }
    }
}
