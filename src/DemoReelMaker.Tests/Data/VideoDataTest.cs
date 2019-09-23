using DemoReelMaker.Data;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace DemoReelMaker.Tests.Data
{
    [TestFixture]
    public class VideoDataTest
    {
        [Test]
        public void Parse_ValidPath_VideoData()
        {
            var actual = VideoData.FromFile("Data/test-list-1.txt");
            Assert.IsNotNull(actual);
            Assert.AreEqual(4, actual.Length);

            var video = actual[0];
            Assert.AreEqual("ixCI4JXUuUE", video.Id);
            Assert.AreEqual("https://youtu.be/ixCI4JXUuUE", video.Url);
            Assert.AreEqual("Oniken", video.Title);
            Assert.AreEqual("Joymasher", video.Description);
            Assert.AreEqual(new TimeSpan(0, 0, 0, 33), video.StartTime);
            Assert.AreEqual(new TimeSpan(0, 0, 0, 15), video.Duration);
            Assert.AreEqual(new TimeSpan(0, 0, 0, 48), video.EndTime);

            video = actual[1];
            Assert.AreEqual("1Gr77r8Ahg0", video.Id);
            Assert.AreEqual("https://youtu.be/1Gr77r8Ahg0", video.Url);
            Assert.AreEqual("Oniken - The Rescue", video.Title);
            Assert.AreEqual("Joymasher", video.Description);
            Assert.AreEqual(new TimeSpan(0, 0, 0, 21), video.StartTime);
            Assert.AreEqual(TimeSpan.MaxValue, video.Duration);
            Assert.AreEqual(TimeSpan.MaxValue, video.EndTime);

            video = actual[2];
            Assert.AreEqual("QhQ9NbuAOwI", video.Id);
            Assert.AreEqual("https://youtu.be/QhQ9NbuAOwI", video.Url);
            Assert.AreEqual("Odallus - The Dark Call", video.Title);
            Assert.AreEqual("Joymasher", video.Description);
            Assert.AreEqual(new TimeSpan(0, 0, 0, 20), video.StartTime);
            Assert.AreEqual(TimeSpan.MaxValue, video.Duration);
            Assert.AreEqual(TimeSpan.MaxValue, video.EndTime);

            video = actual[3];
            Assert.AreEqual("oh_Ur5OKhmQ", video.Id);
            Assert.AreEqual("https://youtu.be/oh_Ur5OKhmQ", video.Url);
            Assert.AreEqual("Blazing Chrome", video.Title);
            Assert.AreEqual("Joymasher", video.Description);
            Assert.AreEqual(new TimeSpan(0, 0, 0, 0), video.StartTime);
            Assert.AreEqual(TimeSpan.MaxValue, video.Duration);
            Assert.AreEqual(TimeSpan.MaxValue, video.EndTime);
        }
    }
}
