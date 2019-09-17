using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiveWallpaperEngine.Models;

namespace LiveWallpaperEngine.Renders
{
    class VideoRender : IWallpaperRender
    {
        public WallpaperType SupportType => WallpaperType.Video;
        public string[] SupportExtensions => new string[] { ".mp4", ".flv", ".blv", ".avi" };
    }
}
