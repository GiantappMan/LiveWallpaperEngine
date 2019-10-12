using System.Collections.Generic;
using LiveWallpaperEngine.Common.Models;
using LiveWallpaperEngineRender.Forms;

namespace LiveWallpaperEngineRender.Renders
{
    class WebRender : BaseRender<VideoControl>
    {
        public static List<WallpaperType> StaticSupportTypes => new List<WallpaperType>()
        {
            new WebWallpaperType(),
        };
        public override List<WallpaperType> SupportTypes => StaticSupportTypes;
    }
}
