using LiveWallpaperEngine.Common;
using LiveWallpaperEngine.Common.Models;
using LiveWallpaperEngine.Common.Renders;
using LiveWallpaperEngineRender.Renders;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;

namespace LiveWallpaperEngineRender
{
    public class WallpaperManager : ILiveWallpaperAPI
    {
        private static System.Timers.Timer _timer;
        private static Dictionary<int, WallpaperModel> _currentWalpapers = new Dictionary<int, WallpaperModel>();

        private WallpaperManager()
        {

        }
        public static LiveWallpaperOptions Options { get; private set; }
        public static WallpaperManager Instance { get; private set; } = new WallpaperManager();
        public static void Initlize()
        {
            //注册render
            RenderFactory.Renders.Add(typeof(VideoRender), VideoRender.StaticSupportTypes);
            RenderFactory.Renders.Add(typeof(WebRender), WebRender.StaticSupportTypes);
            RenderFactory.Renders.Add(typeof(ExeRender), ExeRender.StaticSupportTypes);
        }

        public int GetVolume()
        {
            throw new NotImplementedException();
        }

        public void Pause(params int[] screenIndexs)
        {
            foreach (var index in screenIndexs)
            {
                if (_currentWalpapers.ContainsKey(index))
                {
                    var wallpaper = _currentWalpapers[index];
                    var currentRender = RenderFactory.GetOrCreateRender(wallpaper.Type.DType);
                    currentRender.Pause(screenIndexs);
                }
            }
        }

        public void Resum(params int[] screenIndexs)
        {
            foreach (var index in screenIndexs)
            {
                if (_currentWalpapers.ContainsKey(index))
                {
                    var wallpaper = _currentWalpapers[index];
                    var currentRender = RenderFactory.GetOrCreateRender(wallpaper.Type.DType);
                    currentRender.Resum(screenIndexs);
                }
            }
        }

        public void SetVolume(int v)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {

        }

        public async Task ShowWallpaper(WallpaperModel wallpaper, params int[] screenIndexs)
        {
            if (wallpaper.Type == null)
                wallpaper.Type = RenderFactory.ResoveType(wallpaper.Path);
            if (wallpaper.Type.DType == WalllpaperDefinedType.NotSupport)
                return;

            var currentRender = RenderFactory.GetOrCreateRender(wallpaper.Type.DType);
            await currentRender.ShowWallpaper(wallpaper, screenIndexs);

            foreach (var index in screenIndexs)
            {
                if (!_currentWalpapers.ContainsKey(index))
                    _currentWalpapers.Add(index, wallpaper);
                else
                    _currentWalpapers[index] = wallpaper;
            }
        }

        public void CloseWallpaper(params int[] screenIndexs)
        {
            foreach (var index in screenIndexs)
            {
                if (_currentWalpapers.ContainsKey(index))
                    _currentWalpapers.Remove(index);
            }
            InnerCloseWallpaper(screenIndexs);
        }

        public void InnerCloseWallpaper(params int[] screenIndexs)
        {
            RenderFactory.CacheInstance.ForEach(m => m.CloseWallpaper(screenIndexs));
        }

        public Task SetOptions(LiveWallpaperOptions options)
        {
            Options = options;

            ExplorerMonitor.ExpolrerCreated -= ExplorerMonitor_ExpolrerCreated;
            MaximizedMonitor.AppMaximized -= MaximizedMonitor_AppMaximized;

            if (options.AutoRestartWhenExplorerCrash == true)
                ExplorerMonitor.ExpolrerCreated += ExplorerMonitor_ExpolrerCreated;

            bool enableMaximized = options.ScreenOptions.Exists(m => m.WhenCurrentScreenMaximized != ActionWhenMaximized.Play);
            if (enableMaximized)
                MaximizedMonitor.AppMaximized += MaximizedMonitor_AppMaximized;

            StartTimer(options.AutoRestartWhenExplorerCrash != null && options.AutoRestartWhenExplorerCrash.Value || enableMaximized);

            return Task.CompletedTask;
        }

        private void StartTimer(bool enable)
        {
            if (enable)
            {
                if (_timer == null)
                    _timer = new System.Timers.Timer(1000);

                _timer.Elapsed -= Timer_Elapsed;
                _timer.Elapsed += Timer_Elapsed;
                _timer.Start();
            }
            else
            {
                _timer.Elapsed -= Timer_Elapsed;
                _timer.Stop();
            }
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            _timer?.Stop();
            ExplorerMonitor.Check();
            MaximizedMonitor.Check();
            _timer?.Start();
        }

        private void ExplorerMonitor_ExpolrerCreated(object sender, EventArgs e)
        {
            //重启
            //Process.Start(Application.ResourceAssembly.Location);
            //Application.Current.Shutdown();
            Application.Restart();
        }

        private void MaximizedMonitor_AppMaximized(object sender, AppMaximizedEvent e)
        {
            var screenIndex = Screen.AllScreens.ToList().IndexOf(e.MaximizedScreen);
            foreach (var item in Options.ScreenOptions)
            {
                if (item.ScreenIndex != screenIndex)
                    continue;

                switch (item.WhenCurrentScreenMaximized)
                {
                    case ActionWhenMaximized.Pause:
                        if (e.Maximized)
                            Pause(screenIndex);
                        else
                            Resum(screenIndex);
                        break;
                    case ActionWhenMaximized.Stop:
                        if (e.Maximized)
                            InnerCloseWallpaper(screenIndex);
                        else
                            if (_currentWalpapers.ContainsKey(screenIndex))
                            _ = ShowWallpaper(_currentWalpapers[screenIndex], screenIndex);
                        break;
                    case ActionWhenMaximized.Play:
                        break;
                }
            }
        }
    }
}
