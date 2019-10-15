using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using DZY.Util.Common.Helpers;
using LiveWallpaperEngine;
using LiveWallpaperEngineAPI;
using LiveWallpaperEngineAPI.Common;
using LiveWallpaperEngineAPI.Renders;

namespace LiveWallpaperEngineRemoteWebRender
{
    /// <summary>
    /// 用node+electron+http api渲染，待c#有更好的库时，再考虑c#渲染
    /// </summary>
    public class ElectronWebRender : IRender
    {
        WebRenderAPI _api;
        Process _renderProcess = null;
        public static List<WallpaperType> StaticSupportTypes => new List<WallpaperType>()
        {
           ConstWallpaperTypes.DefinedType[WalllpaperDefinedType.Web],
        };
        public List<WallpaperType> SupportTypes => StaticSupportTypes;

        public void CloseWallpaper(params int[] screenIndexs)
        {
        }

        public void Dispose()
        {
        }

        public int GetVolume(params int[] screenIndexs)
        {
            return 0;
        }

        public void Pause(params int[] screenIndexs)
        {
        }

        public void Resum(params int[] screenIndexs)
        {
        }

        public void SetVolume(int v, params int[] screenIndexs)
        {
        }

        public async Task ShowWallpaper(WallpaperModel wallpaper, params int[] screenIndexs)
        {
            if (_renderProcess == null || _renderProcess.HasExited)
            {
                var renderAPIPort = NetworkHelper.GetAvailablePort(8080);
                _api = new WebRenderAPI("http://localhost:" + renderAPIPort);
                //_renderProcess = Process.Start(@"D:\gitee\LiveWallpaperEngine\LiveWallpaperWebRender\out\livewallpaperwebrender-win32-ia32\livewallpaperwebrender.exe",
                //   $"--hostPort={renderAPIPort}");
                _renderProcess = Process.Start(@"F:\work\gitee\LiveWallpaperEngine\LiveWallpaperWebRender\out\livewallpaperwebrender-win32-ia32\livewallpaperwebrender.exe",
                  $"--hostPort={renderAPIPort}");
            }

            HostInfo info = await _api.GetInfo();
            while (info == null || !info.Initlized)
            {
                await Task.Delay(1000);
                info = await _api.GetInfo();
            }

            var hosts = await _api.ShowWallpaper(wallpaper, screenIndexs);
            foreach (var scIndex in screenIndexs)
            {
                IntPtr windowHandle = hosts[scIndex];
                WallpaperHelper.GetInstance(scIndex).SendToBackground(windowHandle);
            }
        }
    }
}
