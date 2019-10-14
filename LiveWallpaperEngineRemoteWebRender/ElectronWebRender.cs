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
        Process _renderProcess = null;
        public static List<WallpaperType> StaticSupportTypes => new List<WallpaperType>()
        {
           ConstWallpaperTypes.DefinedType[WalllpaperDefinedType.Web],
        };
        public List<WallpaperType> SupportTypes => StaticSupportTypes;

        public void CloseWallpaper(params int[] screenIndexs)
        {
            throw new System.NotImplementedException();
        }

        public void Dispose()
        {
            throw new System.NotImplementedException();
        }

        public int GetVolume(params int[] screenIndexs)
        {
            throw new System.NotImplementedException();
        }

        public void Pause(params int[] screenIndexs)
        {
            throw new System.NotImplementedException();
        }

        public void Resum(params int[] screenIndexs)
        {
            throw new System.NotImplementedException();
        }

        public void SetVolume(int v, params int[] screenIndexs)
        {
            throw new System.NotImplementedException();
        }

        public async Task ShowWallpaper(WallpaperModel wallpaper, params int[] screenIndexs)
        {
            if (_renderProcess == null || _renderProcess.HasExited)
            {
                var renderAPIPort = NetworkHelper.GetAvailablePort(8080);
                _renderProcess = Process.Start("", renderAPIPort.ToString());
            }

            foreach (var scIndex in screenIndexs)
            {
                IntPtr windowHandle = await GetHostFromRemote(scIndex);
                WallpaperHelper.GetInstance(scIndex).SendToBackground(windowHandle);
            }
            throw new System.NotImplementedException();
        }

        private Task<IntPtr> GetHostFromRemote(int scIndex)
        {
            throw new NotImplementedException();
        }
    }
}
