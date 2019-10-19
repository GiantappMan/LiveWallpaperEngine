using Grpc.Net.Client;
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
            CreateClient();
        }

        private void CreateClient()
        {
            //使用http协议
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            var channel = GrpcChannel.ForAddress("http://127.0.0.1:8080");
            _client = new API.APIClient(channel);
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

        public async Task ShowWallpaper(WallpaperModel wallpaper, params uint[] screenIndexs)
        {
            try
            {
                var cts = new CancellationTokenSource();
                cts.CancelAfter(TimeSpan.FromSeconds(10));

                var para = new ShowWallpaperRequest() { Wallpaper = wallpaper };
                para.ScreenIndexs.AddRange(screenIndexs);
                _ = await _client.ShowWallpaperAsync(para);
            }
            catch (Exception)
            {
            }
        }

        public async void CloseWallpaper(params uint[] screenIndexs)
        {
            try
            {
                var cts = new CancellationTokenSource();
                cts.CancelAfter(TimeSpan.FromSeconds(10));

                var para = new CloseWallpaperRequest();
                para.ScreenIndexs.AddRange(screenIndexs);
                var reply = await _client.CloseWallpaperAsync(para);
            }
            catch (Exception)
            {
            }
        }

        public async Task SetOptions(LiveWallpaperOptions setting)
        {
            try
            {
                var cts = new CancellationTokenSource();
                cts.CancelAfter(TimeSpan.FromSeconds(10));

                var reply = await _client.SetOptionsAsync(setting);
            }
            catch (Exception ex)
            {
            }
        }

        #endregion
    }
}
