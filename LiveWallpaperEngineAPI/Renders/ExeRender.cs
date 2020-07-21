using Giantapp.LiveWallpaper.Engine.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Giantapp.LiveWallpaper.Engine.Renders
{
    public class ExeRender : ExternalProcessRender
    {
        public ExeRender() : base(WallpaperType.Exe, new List<string>() { ".exe" })
        {

        }

        public override async Task ShowWallpaper(WallpaperModel wallpaper, params string[] screens)
        {
            await base.ShowWallpaper(wallpaper, screens);

            foreach (var item in _currentWallpapers)
            {
                DesktopMouseEventReciver.HTargetWindows.Add(item.Value.Handle);
            }
            await Task.Run(DesktopMouseEventReciver.Start);
        }

        public override void CloseWallpaper(params string[] screens)
        {
            base.CloseWallpaper(screens);

            if (_currentWallpapers.Count == 0)
                DesktopMouseEventReciver.Stop();
        }
    }
}
