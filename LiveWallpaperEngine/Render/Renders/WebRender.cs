using System.Collections.Generic;
using LiveWallpaperEngine;
using LiveWallpaperEngineRender.Forms;

namespace LiveWallpaperEngineRender.Renders
{
    class WebRender : BaseRender<VideoControl>
    {
        public static List<WallpaperType> StaticSupportTypes => new List<WallpaperType>()
        {
           ConstWallpaperTypes.DefinedType[WalllpaperDefinedType.Web],
        };
        public override List<WallpaperType> SupportTypes => StaticSupportTypes;
    }
}
