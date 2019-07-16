using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;

namespace  DemoReelMaker.Data
{
    /// <summary>
    /// Represents a data for a video, things like url, title, description, duration, etc.
    /// </summary>
    [DebuggerDisplay("{Title}: {Url}")]
    public class VideoData
    {
        private static readonly Regex _getVideoIdRegex = new Regex(@"(?<id>[a-z0-9\-_]+)$", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled);

        /// <summary>
        /// Initializes a new instance of the <see cref="VideoData"/> class.
        /// </summary>
        /// <param name="id">The identifier.</param>
        public VideoData(string id)
        {
            Id = id;
        }

        /// <summary>
        /// Prevents a default instance of the <see cref="VideoData"/> class from being created.
        /// </summary>
        private VideoData()
        {
        }

        /// <summary>
        /// Gets the id.
        /// </summary>
        public string Id { get; private set; }

        /// <summary>
        /// Gets or sets the URL.
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the start time.
        /// </summary>
        public TimeSpan StartTime { get; set; }

        /// <summary>
        /// Gets or sets the duration.
        /// </summary>
        public TimeSpan Duration { get; set; }

        /// <summary>
        /// Gets the end time.
        /// </summary>
        public TimeSpan EndTime => StartTime + Duration;

        /// <summary>
        /// Gets or sets the downloaded file path.
        /// </summary>        
        public string DownloadedFilePath { get; set; }

        /// <summary>
        /// Gets the name of the downloaded file.
        /// </summary>
        public string DownloadedFileName => Path.GetFileNameWithoutExtension(DownloadedFilePath);

        /// <summary>
        /// Reads the videos data from the specified file.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
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