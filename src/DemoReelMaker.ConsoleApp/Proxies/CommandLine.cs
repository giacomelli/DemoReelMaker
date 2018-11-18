using System.Diagnostics;
using DemoReelMaker.ConsoleApp.Logging;

namespace DemoReelMaker.ConsoleApp.Proxies
{
    public static class CommandLine
    {
        public static void RunCommand(string commandName, string arguments)
        {
            var info = new ProcessStartInfo
            {
                FileName = commandName,
                Arguments = arguments
            };

            //Logger.Log($"{commandName} {arguments}");
            var ps = Process.Start(info);
            ps.WaitForExit();
        }
    }
}
