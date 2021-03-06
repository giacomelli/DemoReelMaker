﻿using System;
using System.Collections.Generic;
using System.IO;
using DemoReelMaker.ConsoleApp.Data;

namespace DemoReelMaker.ConsoleApp.Proxies
{
    public class YouTube : ProxyBase
    {
        public void Download(IEnumerable<VideoData> videos)
        {
            Log("Downwloading videos ...");

            var videoNumber = 1;

            foreach (var video in videos)
            {
                var file = Output.GetVideoFile(video);

                if (file == null)
                {
                    Log($"Downloading {video.Title} ...");
                    Output.RunOn("youtube-dl", $"--no-warnings {video.Url}");

                    file = Output.GetVideoFile(video);


                    if (string.IsNullOrEmpty(file))
                    {
                        throw new InvalidOperationException($"Did not find a video file with Id '{video.Id}' on folder on output folder. Please, check if Url video is ok.");
                    }


                    // Rename the file to a ordered one and set info back to VideoData.
                    var orderedFileNumber = Path.Combine(Path.GetDirectoryName(file), $"{videoNumber:00}-{Path.GetFileName(file)}");
                    File.Move(file, orderedFileNumber);
                    file = orderedFileNumber;
                }
                else
                {
                    Log($"Video {video.Title} already download.");
                }

                video.DownloadedFilePath = file;
                videoNumber++;
            }
        }
    }
}