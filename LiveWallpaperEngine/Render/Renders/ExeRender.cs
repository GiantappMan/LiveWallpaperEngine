using LiveWallpaperEngine;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LiveWallpaperEngineRender.Renders
{
    public class ExeRender : IRender
    {
        public List<WallpaperType> SupportTypes => StaticSupportTypes;
        public static List<WallpaperType> StaticSupportTypes => new List<WallpaperType>()
        {
           ConstWallpaperTypes.DefinedType[WalllpaperDefinedType.Exe],
        };

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public int GetVolume(params int[] screenIndexs)
        {
            throw new NotImplementedException();
        }

        public void Pause(params int[] screenIndexs)
        {
            throw new NotImplementedException();
        }

        public void Resum(params int[] screenIndexs)
        {
            throw new NotImplementedException();
        }

        public void SetVolume(int v, params int[] screenIndexs)
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
