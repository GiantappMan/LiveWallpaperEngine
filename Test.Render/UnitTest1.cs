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
        public void TestRenderPause()
        {
            IRender render = GetVideoRender();
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
            IRender render = GetVideoRender();
            render.Play("ss");
            Assert.AreEqual(render.Playing, true);
            render.Stop();
            Assert.AreEqual(render.Playing, false);
            Assert.AreEqual(render.Paused, false);
        }

        [TestMethod]
        public void TestRenderDispose()
        {
            IRender render = GetVideoRender();
            render.Play("ss");
            Assert.AreEqual(render.Playing, true);

            Assert.AreEqual(render.RenderDisposed, false);

            render.CloseRender();
            Assert.AreEqual(render.Paused, false);
            Assert.AreEqual(render.Playing, false);

            Assert.AreEqual(render.RenderDisposed, true);
        }

        private IRender GetVideoRender()
        {
            IRender render = new VideoRender();
            return render;
        }
    }
}
