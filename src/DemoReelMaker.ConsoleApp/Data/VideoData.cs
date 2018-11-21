using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;

namespace DemoReelMaker.ConsoleApp.Data
{
    [DebuggerDisplay("{Title}: {Url}")]
    public class VideoData
    {
        private static readonly Regex _getVideoIdRegex = new Regex(@"(?<id>[a-z0-9\-_]+)$", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled);

        public string Id { get; private set; }
        public string Url { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan Duration { get; set; }
        public TimeSpan EndTime => StartTime + Duration;
        public string DownloadedFilePath { get; set; }
        public string DownloadedFileName => Path.GetFileNameWithoutExtension(DownloadedFilePath);

        public static VideoData[] FromFile(string path)
        {
            var videos = new List<VideoData>();
            var lines = File.ReadAllLines(path);

            foreach(var line in lines)
            {
                if (line.StartsWith("#", StringComparison.OrdinalIgnoreCase) || String.IsNullOrWhiteSpace(line))
                    continue;

                var dataLine = line.Split(';');
                var video = new VideoData
                {
                    Url = dataLine[0],
                    Title = dataLine[1],
                    Description = dataLine[2],
                    StartTime = TimeSpan.ParseExact(dataLine[3], "hh\\:mm\\:ss", CultureInfo.InvariantCulture),
                    Duration = TimeSpan.ParseExact(dataLine[4], "hh\\:mm\\:ss", CultureInfo.InvariantCulture)
                };

                video.Id = _getVideoIdRegex.Match(video.Url).Groups["id"].Value;

                videos.Add(video);
            }

            return videos.ToArray();
        }
    }
}
