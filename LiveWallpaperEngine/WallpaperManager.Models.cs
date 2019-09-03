using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveWallpaperEngine
{

    public enum WallpaperType
    {
        Automatic,
        Video,
        Image,
        Html,
        Exe
    }

    public struct WallpaperSetting
    {
    }

    public struct Wallpaper
    {
        public WallpaperType Type { get; set; }

        public string Path { get; set; }
        public WallpaperSetting Setting { get; set; }
    }
}
