using LiveWallpaperEngine.Wallpaper.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace LiveWallpaperEngine.Renders
{
    /// <summary>
    /// 用单独进程跑render。因为mpv释放不是很干净，杀进程比较稳妥
    /// </summary>
    public class RemoteRender : IRender
    {
        Process _currentProcess = null;
        public List<WallpaperType> SupportTypes => new List<WallpaperType>()
        {
            new WallpaperType(WallpaperType.DefinedType.Exe,".exe"),
            new WallpaperType(WallpaperType.DefinedType.Video,".mp4", ".flv", ".blv", ".avi"),
            new WallpaperType(WallpaperType.DefinedType.Image,".jpg", ".jpeg", ".png", ".bmp"),
            new WallpaperType(WallpaperType.DefinedType.Web,".html", ".htm")
        };

        public int GetVolume()
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

        public void Dispose()
        {
            _currentProcess?.Close();
            _currentProcess = null;
        }

        public IntPtr GetWindowHandle()
        {
            _currentProcess = Process.Start("");
            return _currentProcess.MainWindowHandle;
        }
    }
}
