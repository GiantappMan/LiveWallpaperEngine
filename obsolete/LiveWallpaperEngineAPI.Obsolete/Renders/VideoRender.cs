using Giantapp.LiveWallpaper.Engine.Forms;
using Giantapp.LiveWallpaper.Engine.Models;
using System.Collections.Generic;

namespace Giantapp.LiveWallpaper.Engine.Renders
{
    class VideoRender : BaseRender<VideoRenderControl>
    {
        public static List<WallpaperType> StaticSupportTypes => new List<WallpaperType>()
        {
            WallpaperType.Video,
            WallpaperType.Image
        };

        public override List<WallpaperType> SupportTypes => StaticSupportTypes;
    }
}
