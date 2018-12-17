using System;
using System.Diagnostics;
using DemoReelMaker.Logging;

namespace  DemoReelMaker.Proxies
{
    public static class CommandLine
    {
        public static void RunCommand(string commandName, string arguments)
        {
            var info = new ProcessStartInfo
            {
                FileName = commandName,
                Arguments = arguments,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            // Logger.Log($"{commandName} {arguments}");
            var ps = Process.Start(info);
            ps.WaitForExit();

            if(ps.ExitCode != 0)
            {
                var error = ps.StandardError.ReadToEnd() ?? ps.StandardOutput.ReadToEnd();

                throw new InvalidOperationException($"Error executing {commandName} {arguments}.\n\n{error}");
            }
        }
    }
}
