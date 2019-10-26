using System.Collections.Generic;
using LiveWallpaperEngineAPI;
using LiveWallpaperEngineAPI.Forms;
using LiveWallpaperEngineAPI.Models;

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
