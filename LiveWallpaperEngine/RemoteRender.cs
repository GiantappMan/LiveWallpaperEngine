using DZY.WinAPI;
using LiveWallpaperEngine.Common;
using LiveWallpaperEngine.Common.Models;
using LiveWallpaperEngine.Common.Renders;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace LiveWallpaperEngine.Renders
{
    /// <summary>
    /// 用单独进程跑render。因为mpv释放不是很干净，杀进程比较稳妥
    /// </summary>
    public class RemoteRender
    {
        static Process _currentProcess = null;
        static IPCHelper _ipc = null;

        public static void Initlize()
        {
            //dpi 相关
            User32WrapperEx.SetThreadAwarenessContext(DPI_AWARENESS_CONTEXT.DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE);
            WallpaperHelper.DoSomeMagic();
        }

        public static int GetVolume()
        {
            throw new NotImplementedException();
        }

        public static void Pause()
        {
            throw new NotImplementedException();
        }

        public static void Resum()
        {
            throw new NotImplementedException();
        }

        public static void SetVolume(int v)
        {
            throw new NotImplementedException();
        }

        public static void Dispose()
        {
            _currentProcess?.Kill();
            _currentProcess = null;
            _ipc.Dispose();
            _ipc = null;
        }

        public static async Task ShowWallpaper(WallpaperModel wallpaper, params int[] screenIndexs)
        {
            if (_ipc == null)
                _ipc = new IPCHelper(IPCHelper.ServerID, IPCHelper.RemoteRenderID);

            if (_currentProcess == null)
            {
                var pList = Process.GetProcessesByName("LiveWallpaperEngineRender");
                _currentProcess = pList?.Length > 0 ? pList[0] : null;
                if (_currentProcess == null)
                {
                    _currentProcess = Process.Start("LiveWallpaperEngineRender.exe");
                    //等待render初始完成
                    await _ipc.Wait<Ready>();
                }
            }

            //显示壁纸
            await _ipc.Send(new InvokeRender()
            {
                InvokeMethod = nameof(IRender.ShowWallpaper),
                Parameters = new object[] { wallpaper, screenIndexs },
            });
        }

        public static async void CloseWallpaper(params int[] screenIndexs)
        {
            await _ipc.Send(new InvokeRender()
            {
                InvokeMethod = nameof(IRender.CloseWallpaper),
                Parameters = new object[] { screenIndexs },
            });
        }
    }
}


