using System.Collections.Generic;
using LiveWallpaperEngine;
using LiveWallpaperEngineAPI.Forms;

namespace LiveWallpaperEngineAPI.Renders
{
    class VideoRender : BaseRender<VideoControl>
    {
        public static List<WallpaperType> StaticSupportTypes => new List<WallpaperType>()
        {
            WallpaperType.Video,
            WallpaperType.Image
        };

        public override List<WallpaperType> SupportTypes => StaticSupportTypes;
    }
}
