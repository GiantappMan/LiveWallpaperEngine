using System.Collections.Generic;
using System.Threading.Tasks;
using LiveWallpaperEngine;
using LiveWallpaperEngineRender.Forms;

namespace LiveWallpaperEngineRender.Renders
{
    class WebRender : IRender
    {
        public static List<WallpaperType> StaticSupportTypes => new List<WallpaperType>()
        {
           ConstWallpaperTypes.DefinedType[WalllpaperDefinedType.Web],
        };
        public List<WallpaperType> SupportTypes => StaticSupportTypes;

        public void CloseWallpaper(params int[] screenIndexs)
        {
            throw new System.NotImplementedException();
        }

        public void Dispose()
        {
            throw new System.NotImplementedException();
        }

        public int GetVolume(params int[] screenIndexs)
        {
            throw new System.NotImplementedException();
        }

        public void Pause(params int[] screenIndexs)
        {
            throw new System.NotImplementedException();
        }

        public void Resum(params int[] screenIndexs)
        {
            throw new System.NotImplementedException();
        }

        public void SetVolume(int v, params int[] screenIndexs)
        {
            throw new System.NotImplementedException();
        }

        public Task ShowWallpaper(WallpaperModel wallpaper, params int[] screenIndex)
        {
            throw new System.NotImplementedException();
        }
    }
}
