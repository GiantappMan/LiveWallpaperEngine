using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiveWallpaperEngine.Models;

namespace LiveWallpaperEngine.Renders
{
    /// <summary>
    /// 用单独进程跑render。因为mpv释放不是很干净，杀进程比较稳妥
    /// </summary>
    public class RemoteRender : IRender
    {
        Process _currentProcess = null;
        public Dictionary<WallpaperType, SupportExtensions> SupportTypes => new Dictionary<WallpaperType, SupportExtensions>()
        {
            { WallpaperType.Exe,new SupportExtensions(){ ".exe" } },
            { WallpaperType.Video,new SupportExtensions(){ ".mp4", ".flv", ".blv", ".avi" } },
            { WallpaperType.Image,new SupportExtensions(){ ".jpg", ".jpeg", ".png", ".bmp" } },
            { WallpaperType.Web,new SupportExtensions(){ ".html", ".htm" } },
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
