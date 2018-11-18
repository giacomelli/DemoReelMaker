using System.IO;
using DemoReelMaker.ConsoleApp.Logging;

namespace DemoReelMaker.ConsoleApp.Proxies
{
    public static class FileSystem
    {
        public static void RemoveFile(string path, string description = null)
        {
            Logger.Log($"Removing {description} {path}...");
            File.Delete(path);
        }
    }
}
