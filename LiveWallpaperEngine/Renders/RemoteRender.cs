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
            new ExeWallpaperType(),
            new VideoWallpaperType(),
            new ImageWallpaperType(),
            new WebWallpaperType()
        };

        public int GetVolume()
        {
            throw new NotImplementedException();
        }

        public void LaunchWallpaper(string path)
        {
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
            _currentProcess = Process.Start("LiveWallpaperEngineRender.exe", $"{0} {@"F:\work\gitee\LiveWallpaperEngine\LiveWallpaperEngine.Samples.NetCore.Test\WallpaperSamples\video.mp4"}");
            return _currentProcess.MainWindowHandle;
        }
    }
}
