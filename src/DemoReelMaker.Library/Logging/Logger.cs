using System;

namespace  DemoReelMaker.Logging
{
    /// <summary>
    /// A very basic logger to console.
    /// </summary>
    public static class Logger
    {
        /// <summary>
        /// Logs the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        public static void Log(string message)
        {
            Console.WriteLine($"[DemoReel] {message}");
        }
    }
}
