using LiveWallpaperEngine.Wallpaper.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace LiveWallpaperEngine.Renders
{
    /// <summary>
    /// 用单独进程跑render。因为mpv释放不是很干净，杀进程比较稳妥
    /// </summary>
    public class RemoteRender : IRender
    {
        Process _currentProcess = null;
        IPCHelper ipc = new IPCHelper();
        public List<WallpaperType> SupportTypes => StaticSupportTypes;
        public static List<WallpaperType> StaticSupportTypes => new List<WallpaperType>()
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
            _currentProcess?.Kill();
            _currentProcess = null;
        }

        public async Task<IntPtr> GetWindowHandle()
        {
            _currentProcess = Process.Start("LiveWallpaperEngineRender.exe", $"{0} {@"F:\work\gitee\LiveWallpaperEngine\LiveWallpaperEngine.Samples.NetCore.Test\WallpaperSamples\video.mp4"}");
            //发送server信息给指定render
            var handle = await ipc.SendAndWait<LaunchWallpaper, RenderInitlized>(new LaunchWallpaper()
            {
                PID = _currentProcess.Id,
                ServerID = ipc.ID
            });
            return handle.Handle;
        }
    }
}


