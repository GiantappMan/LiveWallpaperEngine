using DZY.WinAPI;
using Grpc.Net.Client;
using LiveWallpaperEngine.Common;
using LiveWallpaperEngine.Common.Models;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace LiveWallpaperEngine
{
    /// <summary>
    /// 巨应动态壁纸API
    /// giantapp.cn
    /// </summary>
    public class LiveWallpaper
    {
        Process _currentProcess = null;
        API.APIClient _client = null;

        private LiveWallpaper()
        {
            var channel = GrpcChannel.ForAddress("https://localhost:5001");
            _client = new API.APIClient(channel);

            //dpi 相关
            User32WrapperEx.SetThreadAwarenessContext(DPI_AWARENESS_CONTEXT.DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE);
            WallpaperHelper.DoSomeMagic();
        }

        public static LiveWallpaper Instance { get; private set; } = new LiveWallpaper();

        #region properties

        #endregion

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
            _client = null;
        }

        public async Task ShowWallpaper(WallpaperModel wallpaper, params int[] screenIndexs)
        {
            var cts = new CancellationTokenSource();
            cts.CancelAfter(TimeSpan.FromSeconds(31.5));

            var reply = await _client.ShowWallpaperAsync(new ShowWallpaperRequest() { Name = "test" });
        }

        public async void CloseWallpaper(params int[] screenIndexs)
        {
            var cts = new CancellationTokenSource();
            cts.CancelAfter(TimeSpan.FromSeconds(3.5));

            var reply = await _client.CloseWallpaperAsync(new CloseWallpaperRequest());
        }

        public async Task SetOptions(LiveWallpaperOptions setting)
        {
            var cts = new CancellationTokenSource();
            cts.CancelAfter(TimeSpan.FromSeconds(3.5));

            var reply = await _client.SetOptionsAsync(setting);
        }

        #endregion
    }
}
