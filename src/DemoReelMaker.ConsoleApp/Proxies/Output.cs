using System;
using System.IO;
using System.Linq;
using DemoReelMaker.ConsoleApp.Data;

namespace DemoReelMaker.ConsoleApp.Proxies
{
    public static class Output
    {
        private static readonly string _rootFolder = AppDomain.CurrentDomain.BaseDirectory;
        private static string _outputFolderPath;
      
        public static void Intialize(string videosFileName)
        {
            var subfolderName = Path.GetFileNameWithoutExtension(videosFileName);
            _outputFolderPath = Path.Combine(_rootFolder, "output", subfolderName);
     
            if (!Directory.Exists(_outputFolderPath))
                Directory.CreateDirectory(_outputFolderPath);
        }

        public static void RunOn(string commandName, string arguments)
        {
            try
            {
                Directory.SetCurrentDirectory(_outputFolderPath);
                CommandLine.RunCommand(commandName, arguments);
            }
            finally
            {
                Directory.SetCurrentDirectory(_rootFolder);
            }
        }

        public static string[] GetFiles(string searchPattern = "*.*")
        {
            return Directory.GetFiles(_outputFolderPath, searchPattern);
        }

        public static string GetFile(string searchPattern = "*.*")
        {
            return GetFiles(searchPattern).FirstOrDefault();
        }

        public static string GetVideoFile(VideoData video)
        {
            return String.IsNullOrEmpty(video.DownloadedFilePath)
                         ? Output.GetFile($"*{video.Id}*") : video.DownloadedFilePath;
        }

        public static bool ExistsFile(string searchPattern)
        {
            return GetFiles(searchPattern).Length > 0;
        }

        public static void CopyFile(string oldFileName, string newFileName)
        {
     
            File.Copy(Path.Combine(_outputFolderPath, oldFileName), Path.Combine(_outputFolderPath, newFileName), true);
        }

        public static void DeleteFile(string fileName)
        {
            var path = Path.Combine(_outputFolderPath, fileName);

            if (File.Exists(path))
                File.Delete(path);
        }
    }
}
