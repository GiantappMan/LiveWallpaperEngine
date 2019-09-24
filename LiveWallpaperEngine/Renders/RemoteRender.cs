using LiveWallpaperEngine.Common;
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
        IntPtr _cacheHandler;
        IPCHelper _ipc = null;

        public List<WallpaperType> SupportTypes => StaticSupportTypes;

        public static List<WallpaperType> StaticSupportTypes => new List<WallpaperType>()
        {
            new VideoWallpaperType(),
            new ImageWallpaperType(),
            new WebWallpaperType()
        };

        public RemoteRender()
        {
        }

        public int GetVolume()
        {
            throw new NotImplementedException();
        }

        //public void LaunchWallpaper(string path)
        //{
        //    _ = ipc.Send(new LaunchWallpaper()
        //    {
        //        Path = path
        //    });
        //}

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
            _ipc.Dispose();
            _ipc = null;
        }

        //public async Task<IntPtr> GetWindowHandle()
        //{
        //    if (_cacheHandler != IntPtr.Zero)
        //        return _cacheHandler;
        //    _currentProcess = Process.Start("LiveWallpaperEngineRender.exe");
        //    //发送server信息给指定render
        //    var handle = await ipc.Wait<RenderInitlized>();
        //    _cacheHandler = handle.Handle;
        //    return _cacheHandler;
        //}

        public async Task<IntPtr> LaunchWallpaper(string path, WallpaperType.DefinedType type, int screenIndex)
        {
            if (_ipc == null)
                _ipc = new IPCHelper(IPCHelper.ServerID + screenIndex, IPCHelper.RemoteRenderID + screenIndex);

            if (_currentProcess == null)
            {
                _currentProcess = Process.Start("LiveWallpaperEngineRender.exe", screenIndex.ToString());
                //等待render初始完成
                await _ipc.Wait<Ready>();
            }

            //显示壁纸
            LaunchWallpaperResult handle = await _ipc.SendAndWait<LaunchWallpaper, LaunchWallpaperResult>(new LaunchWallpaper()
            {
                Path = path,
                Type = type
            });
            _cacheHandler = handle.Handle;
            return _cacheHandler;
        }
    }
}


