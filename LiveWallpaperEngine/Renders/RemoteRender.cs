using LiveWallpaperEngine.Common;
using LiveWallpaperEngine.Common.Models;
using LiveWallpaperEngine.Common.Renders;
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

        public async Task ShowWallpaper(WallpaperModel wallpaper, params int[] screenIndexs)
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
                DType = wallpaper.Type.DType,
                InvokeMethod = nameof(IRender.ShowWallpaper),
                Parameters = new object[] { wallpaper, screenIndexs },
            });
        }

        public async void CloseWallpaper(params int[] screenIndexs)
        {
            await _ipc.Send(new InvokeRender()
            {
                InvokeMethod = nameof(IRender.CloseWallpaper),
                Parameters = new object[] { screenIndexs },
            });
        }
    }
}


