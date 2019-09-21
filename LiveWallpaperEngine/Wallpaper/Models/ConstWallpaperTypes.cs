using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveWallpaperEngine.Wallpaper.Models
{
    public class ExeWallpaperType : WallpaperType
    {
        public ExeWallpaperType() : base(DefinedType.Exe, ".exe") { }
    }
    public class VideoWallpaperType : WallpaperType
    {
        public VideoWallpaperType() : base(DefinedType.Video, ".mp4", ".flv", ".blv", ".avi") { }
    }
    public class ImageWallpaperType : WallpaperType
    {
        public ImageWallpaperType() : base(DefinedType.Image, ".jpg", ".jpeg", ".png", ".bmp") { }
    }
    public class WebWallpaperType : WallpaperType
    {
        public WebWallpaperType() : base(DefinedType.Web, ".html", ".htm") { }
    }
}
