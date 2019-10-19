using LiveWallpaperEngine;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LiveWallpaperEngineAPI.Renders
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

        public int GetVolume(params uint[] screenIndexs)
        {
            throw new NotImplementedException();
        }

        public void Pause(params uint[] screenIndexs)
        {
            throw new NotImplementedException();
        }

        public void Resum(params uint[] screenIndexs)
        {
            throw new NotImplementedException();
        }

        public void SetVolume(int v, params uint[] screenIndexs)
        {
            throw new NotImplementedException();
        }

        public Task ShowWallpaper(WallpaperModel wallpaper, params uint[] screenIndex)
        {
            throw new NotImplementedException();
        }

        public void CloseWallpaper(params uint[] screenIndexs)
        {
            throw new NotImplementedException();
        }
    }
}
