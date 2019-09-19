using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiveWallpaperEngine.Models;

namespace LiveWallpaperEngine.Renders
{
    class ImageRender : IWallpaperRender
    {
        public WallpaperType SupportType => WallpaperType.Image;
        public string[] SupportExtensions => new string[] { ".jpg", ".jpeg", ".png", ".bmp" };

        public void Dispose()
        {
        }

        public IntPtr GetWindowHandle()
        {
            return IntPtr.Zero;
        }

        public void LaunchWallpaper(string path)
        {
        }
    }
}
