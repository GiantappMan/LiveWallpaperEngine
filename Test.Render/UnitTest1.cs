using System;
using System.Windows.Forms;
using LiveWallpaperEngine;
using LiveWallpaperEngineRender.Renders;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Test.Render
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestRenderPause()
        {
            IVideoRender render = GetVideoRender();
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
        }

        [TestMethod]
        public void TestRenderStop()
        {
            IVideoRender render = GetVideoRender();
            render.Play("ss");
            Assert.AreEqual(render.Playing, true);
            render.Stop();
            Assert.AreEqual(render.Playing, false);
            Assert.AreEqual(render.Paused, false);
        }

        [TestMethod]
        public void TestRenderDispose()
        {
            var test = Screen.AllScreens;
            IVideoRender render = GetVideoRender();
            render.Play("ss");
            Assert.AreEqual(render.Playing, true);

            Assert.AreEqual(render.RenderDisposed, false);

            render.CloseRender();
            Assert.AreEqual(render.Paused, false);
            Assert.AreEqual(render.Playing, false);

            Assert.AreEqual(render.RenderDisposed, true);
        }

        private IVideoRender GetVideoRender()
        {
            var test = Screen.AllScreens;
            IVideoRender render = new VideoRender();
            return render;
        }
    }
}
