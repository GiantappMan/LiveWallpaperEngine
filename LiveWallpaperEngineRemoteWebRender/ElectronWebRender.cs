using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using DZY.Util.Common.Helpers;
using LiveWallpaperEngineAPI;
using LiveWallpaperEngineAPI.Common;
using LiveWallpaperEngineAPI.Models;
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
        List<uint> _mutedScreenIndex = new List<uint>();
        public static List<WallpaperType> StaticSupportTypes => new List<WallpaperType>()
        {
           WallpaperType.Web,
        };
        public List<WallpaperType> SupportTypes => StaticSupportTypes;

        public async void CloseWallpaper(params uint[] screenIndexs)
        {
            await _api.CloseWallpaper(screenIndexs);
            WallpaperHelper.RefreshWallpaper();
        }

        public void Dispose()
        {
        }

        public int GetVolume(params uint[] screenIndexs)
        {
            return 0;
        }

        public void Pause(params uint[] screenIndexs)
        {
        }

        public void Resum(params uint[] screenIndexs)
        {
        }

        public async void SetVolume(int v, params uint[] screenIndexs)
        {
            lock (this)
            {
                if (v == 0)
                {
                    _mutedScreenIndex.AddRange(screenIndexs);
                    _mutedScreenIndex = _mutedScreenIndex.Distinct().ToList();
                }
                else
                    _mutedScreenIndex = _mutedScreenIndex.Except(screenIndexs).ToList();
            }
            await _api.MuteWindow(_mutedScreenIndex.ToArray());
        }

        public async Task ShowWallpaper(WallpaperModel wallpaper, params uint[] screenIndexs)
        {
            if (_renderProcess == null || _renderProcess.HasExited)
            {
                var renderAPIPort = NetworkHelper.GetAvailablePort(8080);
                _api = new WebRenderAPI("http://localhost:" + renderAPIPort);
                string appDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                string path = Path.Combine(appDir, @"WebRender\livewallpaperwebrender.exe");
                _renderProcess = Process.Start(path, $"--hostPort={renderAPIPort}");
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
