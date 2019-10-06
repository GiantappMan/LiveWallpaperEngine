using System.Collections.Generic;
using LiveWallpaperEngine.Common.Models;
using LiveWallpaperEngineRender.Forms;

namespace LiveWallpaperEngineRender.Renders
{
    class VideoRender : BaseRender<VideoControl>
    {
        public static List<WallpaperType> StaticSupportTypes => new List<WallpaperType>()
        {
            new VideoWallpaperType(),
            new ImageWallpaperType()
        };

        public override List<WallpaperType> SupportTypes => StaticSupportTypes;
    }
}
