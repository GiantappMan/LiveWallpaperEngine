using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using LiveWallpaperEngine.Wallpaper.Models;

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

        public Task<IntPtr> GetWindowHandle()
        {
            throw new NotImplementedException();
        }

        public void LaunchWallpaper(string path)
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
    }
}
