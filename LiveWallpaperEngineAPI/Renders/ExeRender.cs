using Giantapp.LiveWallpaper.Engine.Forms;
using Giantapp.LiveWallpaper.Engine.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Giantapp.LiveWallpaper.Engine.Renders
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
