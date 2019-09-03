using LiveWallpaperEngine;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Test.LiveWallpaperEngine
{
    [TestClass]
    public class UnitTestState
    {
        [TestMethod]
        public void TestShowWallpaper()
        {
            StatusManager statusManager = new StatusManager();
            statusManager.ShowWallpaper(new Wallpaper()
            {
                Path = "test.mp4",
                Type = WallpaperType.Automatic
            }, new int[] { 0 });

            Assert.IsTrue(statusManager.Status.Wallpapers[0].Value.Path == "test.mp4");
        }

        [TestMethod]
        public void TestCloseWallpaper()
        {
            StatusManager statusManager = new StatusManager();
            statusManager.ShowWallpaper(new Wallpaper()
            {
                Path = "test.mp4",
                Type = WallpaperType.Automatic
            }, 0);
            statusManager.ShowWallpaper(new Wallpaper()
            {
                Path = "test1.jpg",
                Type = WallpaperType.Automatic
            }, 1);

            Assert.IsTrue(statusManager.Status.Wallpapers[0].Value.Path == "test.mp4");
            Assert.IsTrue(statusManager.Status.Wallpapers[1].Value.Path == "test1.jpg");

            statusManager.CloseWallpaper(1);
            Assert.IsTrue(statusManager.Status.Wallpapers.Count== 1);
            Assert.IsTrue(statusManager.Status.Wallpapers[0].Value.Path == "test.mp4");
        }
    }
}
