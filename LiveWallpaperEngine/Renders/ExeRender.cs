using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LiveWallpaperEngine.Common.Models;
using LiveWallpaperEngine.Common.Renders;

namespace LiveWallpaperEngine.Renders
{
    public class ExeRender : IRender
    {
        public List<WallpaperType> SupportTypes => StaticSupportTypes;
        public static List<WallpaperType> StaticSupportTypes => new List<WallpaperType>()
        {
            new ExeWallpaperType(),
        };

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public int GetVolume()
        {
            throw new NotImplementedException();
        }

        public void Pause()
        {
            throw new NotImplementedException();
        }

        public void Resum()
        {
            throw new NotImplementedException();
        }

        public void SetVolume(int v)
        {
            throw new NotImplementedException();
        }

        public Task ShowWallpaper(WallpaperModel wallpaper, params int[] screenIndex)
        {
            throw new NotImplementedException();
        }

        public void CloseWallpaper(params int[] screenIndexs)
        {
            throw new NotImplementedException();
        }
    }
}
