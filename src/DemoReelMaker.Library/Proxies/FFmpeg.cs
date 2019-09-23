using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using DemoReelMaker.Data;
using ImageMagick;

namespace  DemoReelMaker.Proxies
{
    /// <summary>
    /// Proxy for FFmpeg.
    /// </summary>
    /// <seealso cref="DemoReelMaker.ProxyBase" />
    public class FFmpeg : ProxyBase
    {
        private readonly IEnumerable<VideoData> _videos;

        /// <summary>
        /// Initializes a new instance of the <see cref="FFmpeg"/> class.
        /// </summary>
        /// <param name="videos">The videos.</param>
        public FFmpeg(IEnumerable<VideoData> videos)
        {
            _videos = videos;
        }

        /// <summary>
        /// Peforms the edits (cut, concat, watermark, cover and thumbnail) on the videos.
        /// </summary>
        /// <returns>True if all edits were successful performed.</returns>
        public bool Edit(EditOptions options = null)
        {
            options = options ?? new EditOptions();

            var cutted = Cut();
            var concatenated = Concat();
            var watermarked = AddWatermark(options);
            var texted = AddTexts();
            var covered = AddCovers(options);
            CreateThumbnail();

            Output.CopyFile("covered-video.mp4", "demo-reel.mp4");

            return cutted || concatenated || watermarked || texted || covered;
        }

        private bool Cut()
        {
            Log("Cutting videos...");
            var anyCut = false;

            foreach (var video in _videos)
            {
                if (Output.ExistsFile($"*{video.Id}*_cutted.mp4"))
                {
                    Log($"Video {video.Title} already cutted.");
                    continue;
                }
                else
                {
                    var durationArg = video.Duration == TimeSpan.MaxValue ? "" : "-t \"{video.Duration}\" ";

                    Log($"Cutting video {video.Title}...");
                    Run($"-i \"{video.DownloadedFilePath}\" -ss \"{video.StartTime}\" {durationArg} -async 1 \"{video.DownloadedFileName}_cutted.mp4\"");
                    anyCut = true;
                }
            }

            return anyCut;
        }

        private bool Concat()
        {
            if (Output.ExistsFile("concatenated-videos.mp4"))
            {
                Log($"Videos already concatenated.");
                return false;
            }
            else
            {
                var cuttedFiles = Output.GetFiles("*_cutted.mp4");

                if (cuttedFiles.Length == 1)
                {
                    Output.CopyFile(cuttedFiles[0], "concatenated-videos.mp4");
                }
                else 
                { 
                    Log("Concatenating videos...");
                    var arguments = new StringBuilder();
                    var inputArguments = new StringBuilder();
                    var filter1Arguments = new StringBuilder();
                    var filter2Arguments = new StringBuilder();

                    filter1Arguments.Append("-filter_complex \"");
                    var videoIndex = 0;
                    var videoLetter = 'a';

                    foreach (var file in Output.GetFiles("*_cutted.mp4"))
                    {
                        inputArguments.AppendFormat($"-i \"{Path.GetFileName(file)}\" ");
                        filter1Arguments.AppendFormat($"[{videoIndex}]scale=1920x1080,setdar=16/9[{videoLetter}];");
                        filter2Arguments.AppendFormat($"[{videoLetter}][{videoIndex}:a]");

                        videoIndex++;
                        videoLetter++;
                    }

                    arguments
                        .Append(inputArguments)
                        .Append(filter1Arguments)
                        .Append(filter2Arguments)
                        .AppendFormat($"concat=n={videoIndex}:v=1:a=1[v][a]\"")
                        .AppendFormat(" -map \"[v]\" -map \"[a]\"")
                        .AppendFormat(" concatenated-videos.mp4");

                    Run(arguments.ToString());
                }

                return true;
            }
        }

        private bool AddWatermark(EditOptions options)
        {
            if (Output.ExistsFile("watermarked-video.mp4"))
            {
                Log($"Videos already watermarked.");
                return false;
            }
            else if(options.WatermarkEnabled)
            {
                Log("Adding watermark...");
                Run("-i concatenated-videos.mp4 -i ../../Resources/watermarks/current.png -filter_complex \"overlay=x=(main_w-overlay_w):y=(main_h-overlay_h)\" watermarked-video.mp4");
                return true;
            }
            else
            {
                Output.CopyFile("concatenated-videos.mp4", "watermarked-video.mp4");
                return true;
            }
        }

        private bool AddTexts()
        {
            if (Output.ExistsFile("texted-video.mp4"))
            {
                Log($"Videos already texted.");
                return false;
            }
            else
            {
                Log("Adding texts...");
           
                var startTime = 0;
                var cmd = new StringBuilder();
                cmd
                    .Append($"-i watermarked-video.mp4")
                    .Append(" -vf format=yuv444p,");

                string PrepareText(string text) => text
                                                        .ToUpperInvariant()
                                                        .Replace('\'', '\u2019')
                                                        .Replace(", ", " - ");

                foreach (var video in _videos)
                {
                    var duration = Convert.ToInt32(video.Duration.TotalSeconds);
                    var endTime = startTime + duration;
                    var title = PrepareText(video.Title);
                    var description = PrepareText(video.Description);

                    cmd.Append($"drawtext=fontfile=../../Resources/fonts/current.ttf\\\\:style=bold:text=\"{title}\":enable='between(t,{startTime},{endTime})':fontcolor=white:fontsize=60:x=150:y=h-line_h-100:box=1:boxcolor=orange@0.7:boxborderw=2,")
                       .Append($"drawtext=fontfile=../../Resources/fonts/current.ttf\\\\:style=italic:text=\"{description}\":enable='between(t,{startTime},{endTime})':fontcolor=white:fontsize=40:x=150:y=h-line_h-50:box=1:boxcolor=orange@0.7:boxborderw=2,");


                    startTime += duration;
                }

                cmd
                    .Append("format=yuv420p")
                    .Append($" -c:v libx264 -c:a copy -movflags +faststart \"texted-video.mp4\"");

                Run(cmd.ToString());

                return true;
            }
        }

        private bool AddCovers(EditOptions options)
        {
            if (Output.ExistsFile("covered-video.mp4"))
            {
                Log($"Videos already covered.");
                return false;
            }
            else if (options.CoversEnabled)
            {
                Log("Adding covers...");
                Run("-loop 1 -t 3 -i \"../../Resources/covers/current.png\" -loop 1 -t 3 -i \"../../Resources/covers/current.png\" -f lavfi -t 3 -i anullsrc -i \"texted-video.mp4\" -filter_complex \"[2:a]asplit[i][e];[0] fade=out:st=0:d=3[0f];[1] fade=in:st=0:d=3[1f];[0f] [i] [3:v] [3:a] [1f] [e] concat=n=3:v=1:a=1[v] [a]\" -map [v] -map [a] covered-video.mp4");
            
                return true;
            }
            else
            {
                Output.CopyFile("texted-video.mp4", "covered-video.mp4");
                return true;
            }
        }

        private void CreateThumbnail()
        {
            using (var images = new MagickImageCollection())
            {
                var montageSettings = new MontageSettings()
                {
                    BackgroundColor = MagickColors.Transparent,
                    Shadow = false,
                    TileGeometry = new MagickGeometry(3, 0),
                    Geometry = new MagickGeometry(0, 0, 640, 0)
                };

                foreach (var file in Output.GetFiles("*_cutted.mp4"))
                {
                    var fileName = Path.GetFileName(file);
                    var thumbnailFile = file.Replace("_cutted.mp4", "_thumbnail.png");
                    Output.DeleteFile(thumbnailFile);
                    var video = _videos.First(v => v.DownloadedFilePath.StartsWith(file.Replace("_cutted.mp4", ""), StringComparison.OrdinalIgnoreCase));
                    var thumbnailPoint = video.Duration.TotalSeconds > 5 ? 5 : video.Duration.TotalSeconds - 1;

                    Run($"-i \"{fileName}\" -ss 00:00:{thumbnailPoint:00} -t 00:00:00.01 \"{thumbnailFile}\"");

                    var thumbnail = new MagickImage(thumbnailFile);
                    images.Add(thumbnail);
                }

                using (IMagickImage result = images.Montage(montageSettings))
                {
                    result.Write(Output.CombinePath("thumbnail.png"));
                }
            }
        }

        private static void Run(string arguments)
        {
            Output.RunOn("ffmpeg", $"-loglevel error {arguments}");
        }
    }
}