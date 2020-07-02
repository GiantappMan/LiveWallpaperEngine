using Giantapp.LiveWallpaper.Engine.Common;
using Giantapp.LiveWallpaper.Engine.Forms;
using Giantapp.LiveWallpaper.Engine.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Giantapp.LiveWallpaper.Engine.Renders
{
    class ExeRender : BaseRender<ExeRenderControl>
    {
        public static List<WallpaperType> StaticSupportTypes => new List<WallpaperType>()
        {
           WallpaperType.Exe,
        };

        public override List<WallpaperType> SupportTypes => StaticSupportTypes;

        public override async Task ShowWallpaper(WallpaperModel wallpaper, params uint[] screenIndexs)
        {
            await base.ShowWallpaper(wallpaper, screenIndexs);

            await Task.Run(DesktopMouseEventReciver.Start);
        }

        public override void CloseWallpaper(params uint[] screenIndexs)
        {
            base.CloseWallpaper(screenIndexs);

            var haveExeWallpaper = WallpaperManager.CurrentWalpapers.Values.FirstOrDefault(m => m.Type == WallpaperType.Exe) != null;
            if (!haveExeWallpaper)
                DesktopMouseEventReciver.Stop();
        }
    }
}
