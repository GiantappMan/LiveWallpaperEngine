using LiveWallpaperEngine.Common;
using LiveWallpaperEngine.Common.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LiveWallpaperEngine.Renders
{
    /// <summary>
    /// 用单独进程跑render。因为mpv释放不是很干净，杀进程比较稳妥
    /// </summary>
    public class RemoteRender : IRender
    {
        Process _currentProcess = null;
        IPCHelper _ipc = null;
        WallpaperHelper wallpaperHelper = null;

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

        public async Task LaunchWallpaper(WallpaperModel wallpaper, int screenIndex)
        {
            if (_ipc == null)
                _ipc = new IPCHelper(IPCHelper.ServerID, IPCHelper.RemoteRenderID);

            if (_currentProcess == null)
            {
                var pList = Process.GetProcessesByName("LiveWallpaperEngineRender");
                _currentProcess = pList?.Length > 0 ? pList[0] : null;
                if (_currentProcess == null)
                {
                    _currentProcess = Process.Start("LiveWallpaperEngineRender.exe", screenIndex.ToString());
                    //等待render初始完成
                    await _ipc.Wait<Ready>();
                }
            }

            //显示壁纸
            LaunchWallpaperResult handle = await _ipc.SendAndWait<LaunchWallpaper, LaunchWallpaperResult>(new LaunchWallpaper()
            {
                Wallpaper = wallpaper,
                ScreenIndex = screenIndex
            });

            if (wallpaperHelper == null)
                wallpaperHelper = new WallpaperHelper(Screen.AllScreens[screenIndex].Bounds);

            wallpaperHelper.SendToBackground(handle.Handle);
        }
    }
}


