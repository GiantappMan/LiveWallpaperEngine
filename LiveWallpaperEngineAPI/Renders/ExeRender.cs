using LiveWallpaperEngineAPI.Forms;
using LiveWallpaperEngineAPI.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace LiveWallpaperEngineAPI.Renders
{
    class ExeRender : BaseRender<ExeRenderControl>
    {
        public static List<WallpaperType> StaticSupportTypes => new List<WallpaperType>()
        {
           WallpaperType.Exe,
        };

        public override List<WallpaperType> SupportTypes => StaticSupportTypes;
    }
}
