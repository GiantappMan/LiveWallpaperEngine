using DZY.WinAPI;
using LiveWallpaperEngine.Common;
using LiveWallpaperEngine.Common.Models;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace LiveWallpaperEngine
{
    /// <summary>
    /// 巨应动态壁纸API
    /// giantapp.cn
    /// </summary>
    public class LiveWallpaper : ILiveWallpaperAPI
    {
        Process _currentProcess = null;
        IPCHelper _ipc = null;

        private LiveWallpaper()
        {
            //dpi 相关
            User32WrapperEx.SetThreadAwarenessContext(DPI_AWARENESS_CONTEXT.DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE);
            WallpaperHelper.DoSomeMagic();
        }

        public static LiveWallpaper Instance { get; private set; } = new LiveWallpaper();

        #region properties

        #endregion

        private async Task<IPCHelper> GetIPCHelper()
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
            return _ipc;
        }

        #region public methods

        public int GetVolume()
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

        public async Task ShowWallpaper(WallpaperModel wallpaper, params int[] screenIndexs)
        {
            var ipc = await GetIPCHelper();
            await ipc.Send(new InvokeILiveWallpaperAPI()
            {
                InvokeMethod = nameof(ILiveWallpaperAPI.ShowWallpaper),
                Parameters = new object[] { wallpaper, screenIndexs }
            });
        }

        public async void CloseWallpaper(params int[] screenIndexs)
        {
            var ipc = await GetIPCHelper();
            await ipc.Send(new InvokeILiveWallpaperAPI()
            {
                InvokeMethod = nameof(ILiveWallpaperAPI.CloseWallpaper),
                Parameters = new object[] { screenIndexs }
            });
        }

        public async Task SetOptions(LiveWallpaperOptions setting)
        {
            var ipc = await GetIPCHelper();
            await ipc.Send(new InvokeILiveWallpaperAPI()
            {
                InvokeMethod = nameof(ILiveWallpaperAPI.SetOptions),
                Parameters = new object[] { setting }
            });
        }

        #endregion
    }
}
