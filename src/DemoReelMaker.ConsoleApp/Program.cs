using System;
using DemoReelMaker.Data;
using DemoReelMaker.Proxies;
using DemoReelMaker.Logging;
using System.Diagnostics;
using System.Threading.Tasks;

namespace  DemoReelMaker
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Logger.Log("Missing YouTube file list argument.");
                Environment.Exit(1);
            }

            var sw = new Stopwatch();
            sw.Start();
            var videosFileName = args[0];
            Output.Initialize(videosFileName);

            // Download from YouTube.
            Logger.Log($"Starting for video list {videosFileName}...");
            var youtube = new YouTube();
            var videos = VideoData.FromFile(videosFileName);

            Logger.Log($"{videos.Length} videos loaded from file.");
            youtube.Download(videos);

            // Videos editing by FFmpeg.
            var ffmpeg = new FFmpeg(videos);
            var options = new EditOptions
            {
                CoversEnabled = false,
                WatermarkEnabled = false,
            };

            if(!ffmpeg.Edit(options))
            {
                Logger.Log("Demo reel video was not generated, because all videos are already done on output folder.");
                Logger.Log("If you want to generated it again remove the sub folder from output folder");
            }

            sw.Stop();
            Logger.Log($"Done in {sw.Elapsed}.");
        }
    }
}
