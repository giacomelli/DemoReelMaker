using System;
using System.Diagnostics;

namespace DemoReelMaker.Proxies
{
    /// <summary>
    /// A proxy to the command-line.
    /// </summary>
    public static class CommandLine
    {
        /// <summary>
        /// Runs the command.
        /// </summary>
        /// <param name="commandName">Name of the command.</param>
        /// <param name="arguments">The arguments.</param>
        /// <exception cref="InvalidOperationException">Error executing {commandName} {arguments}.\n\n{error}</exception>
        public static void RunCommand(string commandName, string arguments)
        {
            var info = new ProcessStartInfo
            {
                FileName = commandName,
                Arguments = arguments,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            using (var ps = Process.Start(info))
            {
                ps.ReadOutputToEnd(out string standardOutput, out string standardError);
                ps.WaitForExit();

                if (ps.ExitCode != 0)
                {
                    var error = standardError ?? standardOutput;

                    throw new InvalidOperationException($"Error executing {commandName} {arguments}.\n\n{error}");
                }
            }
        }
    }
}
