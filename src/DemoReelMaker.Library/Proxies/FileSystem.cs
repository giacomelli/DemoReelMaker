using System.IO;
using  DemoReelMaker.Logging;

namespace  DemoReelMaker.Proxies
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
