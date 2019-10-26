using System;
using System.Collections.Generic;
using System.Text;

namespace LiveWallpaperEngineAPI.Models
{
    public class WallpaperModel
    {
        public WallpaperType Type { get; set; }
        public string Path { get; set; }
        public WallpaperInfo Info { get; set; }
    }
}
