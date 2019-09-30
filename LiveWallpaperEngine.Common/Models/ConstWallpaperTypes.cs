using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveWallpaperEngine.Common.Models
{
    public class ExeWallpaperType : WallpaperType
    {
        public ExeWallpaperType() : base(WalllpaperDefinedType.Exe, ".exe") { }
    }
    public class VideoWallpaperType : WallpaperType
    {
        public VideoWallpaperType() : base(WalllpaperDefinedType.Video, ".mp4", ".flv", ".blv", ".avi") { }
    }
    public class ImageWallpaperType : WallpaperType
    {
        public ImageWallpaperType() : base(WalllpaperDefinedType.Image, ".jpg", ".jpeg", ".png", ".bmp") { }
    }
    public class WebWallpaperType : WallpaperType
    {
        public WebWallpaperType() : base(WalllpaperDefinedType.Web, ".html", ".htm") { }
    }
}
