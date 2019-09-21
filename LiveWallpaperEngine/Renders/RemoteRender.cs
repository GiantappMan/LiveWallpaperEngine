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
        string _chidrenProcessIpcId = Guid.NewGuid().ToString();
        IntPtr _cacheHandler;

        IPCHelper ipc = null;
        public List<WallpaperType> SupportTypes => StaticSupportTypes;
        public static List<WallpaperType> StaticSupportTypes => new List<WallpaperType>()
        {
            new ExeWallpaperType(),
            new VideoWallpaperType(),
            new ImageWallpaperType(),
            new WebWallpaperType()
        };

        public RemoteRender()
        {
            ipc = new IPCHelper(Guid.NewGuid().ToString(), _chidrenProcessIpcId);
        }

        public int GetVolume()
        {
            throw new NotImplementedException();
        }

        public void LaunchWallpaper(string path)
        {
            _ = ipc.Send(new LaunchWallpaper()
            {
                Path = path
            });
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
            ipc.Dispose();
            ipc = null;
        }

        public async Task<IntPtr> GetWindowHandle()
        {
            if (_cacheHandler != IntPtr.Zero)
                return _cacheHandler;
            _currentProcess = Process.Start("LiveWallpaperEngineRender.exe", $"{ipc.ID} {_chidrenProcessIpcId}");
            //发送server信息给指定render
            var handle = await ipc.Wait<RenderInitlized>();
            _cacheHandler = handle.Handle;
            return _cacheHandler;
        }
    }
}


