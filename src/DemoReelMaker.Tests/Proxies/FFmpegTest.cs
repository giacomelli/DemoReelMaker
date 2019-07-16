using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DemoReelMaker.Data;
using DemoReelMaker.Proxies;
using NUnit.Framework;

namespace DemoReelMaker.Tests.Proxies
{
    [TestFixture]
    public class FFmpegTest
    {
        private VideoData[] _videos;
     
        [SetUp]
        public void Setup()
        {
            Output.Initialize("FFmpegTest");
            Output.Clear();
            
            _videos = new VideoData[]
            {  
                CreateVideoData("zHUnJrvzS14", "Magenta Arcade", "Long Hat House"),
                CreateVideoData("IAu4oJzzNDI", "Bad Rats: A Vingança dos Ratos", "INVENT4 Entertainment"),
                CreateVideoData("ktZTVnhOQdw", "CORRE, CHICO!", "Imax Games")
            };

            var target = new YouTube();
            target.Download(_videos);
        }

        [Test]
        public void Edit_1Video_DemoReelDone()
        {
            AssertEdit(_videos.Take(1).ToArray());
        }

        [Test]
        public void Edit_AllVideos_DemoReelDone()
        {
            AssertEdit(_videos);
        }

        private VideoData CreateVideoData(string id, string title, string description)
        {               
            return new VideoData(id)
            {
                Url = $"https://www.youtube.com/watch?v={id}",
                Title = title,
                Description = description,
                StartTime = TimeSpan.Zero,
                Duration = TimeSpan.FromSeconds(5)
            };
        }

        private void AssertEdit(params VideoData[] inputVideos)
        {
            var target = new FFmpeg(inputVideos);
            var actual = target.Edit();

            Assert.IsTrue(actual);

            foreach (var video in inputVideos)
            {
                var fileName = Path.GetFileNameWithoutExtension(video.DownloadedFileName);
                Assert.AreEqual(1, Output.GetFiles($"{fileName}_cutted.mp4").Length, "Missing cutted video file");
                Assert.AreEqual(1, Output.GetFiles("concatenated-videos.mp4").Length, "Missing concatenated video file");
                Assert.AreEqual(1, Output.GetFiles("watermarked-video.mp4").Length, "Missing watermarked video file");
                Assert.AreEqual(1, Output.GetFiles("texted-video.mp4").Length, "Missing texted video file");
                Assert.AreEqual(1, Output.GetFiles("covered-video.mp4").Length, "Missing covered video file");
                Assert.AreEqual(1, Output.GetFiles("demo-reel.mp4").Length, "Missing demo reel video file");
                Assert.AreEqual(1, Output.GetFiles($"{fileName}_thumbnail.png").Length, "Missing thumnail video file");
                Assert.AreEqual(1, Output.GetFiles("thumbnail.png").Length, "Missing demo rell thumbnail file");
            }
        }
    }
}
