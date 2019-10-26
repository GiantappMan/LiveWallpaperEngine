using System;
using System.Collections.Generic;
using System.Text;

namespace LiveWallpaperEngineAPI.Models
{
    public class WallpaperModel
    {
        public WallpaperType Type { get; internal set; }
        public string Path { get; internal set; }
        public WallpaperInfo Info { get; internal set; }
    }
}
