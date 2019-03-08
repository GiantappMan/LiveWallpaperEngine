using System;
using LiveWallpaperEngine;
using LiveWallpaperEngineRender.Renders;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Test.Render
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestVideoRender()
        {
            IRender render = GetVideoRender();
            TestRender(render);
        }

        private void TestRender(IRender render)
        {
            Assert.AreEqual(render.Paused, false);
            Assert.AreEqual(render.Playing, false);

            render.Play("ss");
            Assert.AreEqual(render.Playing, true);

            render.Pause();
            Assert.AreEqual(render.Playing, false);
            Assert.AreEqual(render.Paused, true);

            render.Resume();
            Assert.AreEqual(render.Playing, true);
            Assert.AreEqual(render.Paused, false);

            render.Stop();
            Assert.AreEqual(render.Playing, false);
            Assert.AreEqual(render.Paused, false);

            render.CloseRender();
            Assert.AreEqual(render.Paused, false);
            Assert.AreEqual(render.Playing, false);
        }

        private IRender GetVideoRender()
        {
            IRender render = new VideoRender();
            return render;
        }
    }
}
