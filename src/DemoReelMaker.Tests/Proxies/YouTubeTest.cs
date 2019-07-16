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
    public class YouTubeTest
    {   
        [Test]
        public void Download_Videos_Downloaded()
        {
            Output.Initialize();
            Output.Clear();

            var target = new YouTube();
            target.Download(new VideoData[]
            {
                new VideoData("JxvOshwNdfE") { Url = "https://www.youtube.com/watch?v=JxvOshwNdfE", Title = "Test1" },
                new VideoData("YljjAkebxiE") { Url = "https://www.youtube.com/watch?v=YljjAkebxiE", Title = "Test2" }
            });

            var files = Output.GetFiles();
            Assert.AreEqual(2, files.Length);
        }
    }
}
