using LiveWallpaperEngine;
using LiveWallpaperEngine.Wallpaper.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Test.LiveWallpaperEngine
{
    [TestClass]
    public class UnitTestState
    {
        [TestMethod]
        public void TestShowWallpaper()
        {
            LiveWallpaper.Show(new WallpaperModel()
            {
                Path = "test.mp4",
            }, 0);

            Assert.IsTrue(LiveWallpaper.Status.Wallpapers[0].Value.Path == "test.mp4");
            Assert.IsTrue(LiveWallpaper.Status.Wallpapers[0].Value.Type.DType == WallpaperType.DefinedType.Video);
        }

        [TestMethod]
        public void TestCloseWallpaper()
        {
            LiveWallpaper.Show(new WallpaperModel()
            {
                Path = "test.mp4",
            }, 0);
            LiveWallpaper.Show(new WallpaperModel()
            {
                Path = "test1.jpg",
            }, 1);

            Assert.IsTrue(LiveWallpaper.Status.Wallpapers[0].Value.Path == "test.mp4");
            Assert.IsTrue(LiveWallpaper.Status.Wallpapers[1].Value.Path == "test1.jpg");
            Assert.IsTrue(LiveWallpaper.Status.Wallpapers[0].Value.Type.DType == WallpaperType.DefinedType.Video);
            Assert.IsTrue(LiveWallpaper.Status.Wallpapers[1].Value.Type.DType == WallpaperType.DefinedType.Image);

            LiveWallpaper.Close(1);
            Assert.IsTrue(LiveWallpaper.Status.Wallpapers.Count == 1);
            Assert.IsTrue(LiveWallpaper.Status.Wallpapers[0].Value.Path == "test.mp4");
        }
    }
}
