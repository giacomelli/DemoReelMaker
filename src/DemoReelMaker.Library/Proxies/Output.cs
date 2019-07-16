using System;
using System.IO;
using System.Linq;
using DemoReelMaker.Data;

namespace  DemoReelMaker.Proxies
{
    /// <summary>
    /// Proxy for the output folder were download, intermediate edited vidoes and final demo reel will be write.
    /// </summary>
    public static class Output
    {
        private static readonly string _rootFolder = AppDomain.CurrentDomain.BaseDirectory;
        private static string _outputFolderPath;
      
        static Output()
        {
            Initialize();
        }

        /// <summary>
        /// Initializes the proxy.
        /// </summary>
        /// <param name="videosFileName">Name of the videos file.</param>
        public static void Initialize(string videosFileName = null)
        {
            videosFileName = videosFileName ?? Guid.NewGuid().ToString();
            var subfolderName = Path.GetFileNameWithoutExtension(videosFileName);
            _outputFolderPath = Path.Combine(_rootFolder, "output", subfolderName);
     
            if (!Directory.Exists(_outputFolderPath))
                Directory.CreateDirectory(_outputFolderPath);
        }

        /// <summary>
        /// Clear the output folder.
        /// </summary>
        public static void Clear()
        {
            Directory.Delete(_outputFolderPath, true);
            Initialize(_outputFolderPath);
        }

        /// <summary>
        /// Combines the specified path with the output folder path.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <returns>The combined path.</returns>
        public static string CombinePath(string filename)
        {
            return Path.Combine(_outputFolderPath, filename);
        }

        /// <summary>
        /// Runs the specified command on command-line using the output folder as working directory.
        /// </summary>
        /// <param name="commandName">Name of the command.</param>
        /// <param name="arguments">The arguments.</param>
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

        /// <summary>
        /// Gets the files from output folder.
        /// </summary>
        /// <param name="searchPattern">The search pattern.</param>
        /// <returns>The files.</returns>
        public static string[] GetFiles(string searchPattern = "*.*")
        {
            return Directory.GetFiles(_outputFolderPath, searchPattern);
        }

        /// <summary>
        /// Gets the first file from output folder.
        /// </summary>
        /// <param name="searchPattern">The search pattern.</param>
        /// <returns>The file.</returns>
        public static string GetFile(string searchPattern = "*.*")
        {
            return GetFiles(searchPattern).FirstOrDefault();
        }

        /// <summary>
        /// Gets the video file of the specified video data.
        /// </summary>
        /// <param name="video">The video.</param>
        /// <returns>The video file.</returns>
        public static string GetVideoFile(VideoData video)
        {
            return String.IsNullOrEmpty(video.DownloadedFilePath)
                         ? Output.GetFile($"*{video.Id}*") : video.DownloadedFilePath;
        }

        /// <summary>
        /// Verifies if the file exists.
        /// </summary>
        /// <param name="searchPattern">The search pattern.</param>
        /// <returns>True if file exists.</returns>
        public static bool ExistsFile(string searchPattern)
        {
            return GetFiles(searchPattern).Length > 0;
        }

        /// <summary>
        /// Copies the file.
        /// </summary>
        /// <param name="oldFileName">Old name of the file.</param>
        /// <param name="newFileName">New name of the file.</param>
        /// <returns>The new file path.</returns>
        public static string CopyFile(string oldFileName, string newFileName)
        {
            string newFilePath = Path.Combine(_outputFolderPath, newFileName);

            if (Path.IsPathRooted(oldFileName))
                File.Copy(oldFileName, newFilePath, true);
            else
                File.Copy(Path.Combine(_outputFolderPath, oldFileName), newFilePath, true);

            return newFilePath;
        }

        /// <summary>
        /// Deletes the file.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        public static void DeleteFile(string fileName)
        {
            var path = Path.Combine(_outputFolderPath, fileName);

            if (File.Exists(path))
                File.Delete(path);
        }
    }
}
