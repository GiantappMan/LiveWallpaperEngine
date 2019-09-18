using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiveWallpaperEngine.Models;

namespace LiveWallpaperEngine.Renders
{
    class WebRender : IWallpaperRender
    {
        public WallpaperType SupportType => WallpaperType.Web;
        public string[] SupportExtensions => new string[] { ".html", ".htm" };

        public void Close()
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
