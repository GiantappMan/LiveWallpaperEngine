using LiveWallpaperEngineAPI.Common;
using LiveWallpaperEngineAPI.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace LiveWallpaperEngineAPI.Renders
{
    public class ExeRender : IRender
    {
        public List<WallpaperType> SupportTypes => StaticSupportTypes;
        public static List<WallpaperType> StaticSupportTypes => new List<WallpaperType>()
        {
           WallpaperType.Exe,
        };

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public int GetVolume(params uint[] screenIndexs)
        {
            throw new NotImplementedException();
        }

        public void Pause(params uint[] screenIndexs)
        {
        }

        public void Resum(params uint[] screenIndexs)
        {
        }

        public void SetVolume(int v, params uint[] screenIndexs)
        {
          
        }

        public Task ShowWallpaper(WallpaperModel wallpaper, params uint[] screenIndexs)
        {
            foreach (var index in screenIndexs)
            {
                var process = Process.Start(wallpaper.Path);
                WallpaperHelper.GetInstance(index).SendToBackground(process.MainWindowHandle);
            }
            return Task.CompletedTask;
        }

        public void CloseWallpaper(params uint[] screenIndexs)
        {
        }
    }
}
