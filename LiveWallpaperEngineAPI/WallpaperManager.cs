using Giantapp.LiveWallpaper.Engine.Renders;
using Giantapp.LiveWallpaper.Engine.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Threading;
using static Giantapp.LiveWallpaper.Engine.ScreenOption;

namespace Giantapp.LiveWallpaper.Engine
{
    public static class WallpaperManager
    {
        #region field

        private static System.Timers.Timer _timer;
        private static Dispatcher _uiDispatcher;

        #endregion

        #region property

        public static string[] Screens { get; private set; }
        public static LiveWallpaperOptions Options { get; private set; }

        //Dictionary<DeviceName，WallpaperModel>
        public static Dictionary<string, (WallpaperModel WPModel, bool IsStopedTemporary)> CurrentWalpapers { get; private set; } = new Dictionary<string, (WallpaperModel WPModel, bool IsStopedTemporary)>();

        #endregion

        #region public

        public static void Initlize(Dispatcher dispatcher)
        {
            RenderFactory.Renders.Add(new ExeRender());
            RenderFactory.Renders.Add(new VideoRender());
            _uiDispatcher = dispatcher;
            Screens = Screen.AllScreens.Select(m => m.DeviceName).ToArray();
        }

        internal static void UIInvoke(Action a)
        {
            _uiDispatcher.Invoke(a);
        }

        public static Task<List<WallpaperModel>> GetWallpapers(string dir)
        {
            throw new NotImplementedException();
        }

        public static async Task<bool> DeleteWallpaperPack(string absolutePath)
        {
            throw new NotImplementedException();
        }

        public static async Task<WallpaperModel> UpdateWallpaper(WallpaperModel source, WallpaperModel newWP)
        {
            throw new NotImplementedException();
        }

        public static async Task<WallpaperModel> CreateWallpaper(string path)
        {
            throw new NotImplementedException();
        }

        public static async Task ShowWallpaper(WallpaperModel wallpaper, params string[] screens)
        {
            if (screens.Length == 0)
                screens = Screens;

            IRender currentRender;
            if (wallpaper.Type == null)
                currentRender = RenderFactory.GetRenderByExtension(Path.GetExtension(wallpaper.Path));
            else
                currentRender = RenderFactory.GetRender(wallpaper.Type.Value);

            if (currentRender == null)
                if (wallpaper.Type == null)
                    throw new ArgumentException("Unsupported wallpaper type");


            foreach (var screenItem in screens)
            {
                //壁纸为空
                if (!CurrentWalpapers.ContainsKey(screenItem))
                {
                    await currentRender.ShowWallpaper(wallpaper, screenItem);

                    CurrentWalpapers.Add(screenItem, (wallpaper, false));
                }
                //有壁纸，但是路径不一样或者壁纸已经临时关闭
                else
                {
                    var tmpWallpaper = CurrentWalpapers[screenItem];
                    if (tmpWallpaper.WPModel.Path != wallpaper.Path || tmpWallpaper.IsStopedTemporary)
                    {
                        //关闭之前的壁纸
                        CloseWallpaper(screenItem);
                        await currentRender.ShowWallpaper(wallpaper, screenItem);

                        CurrentWalpapers[screenItem] = (wallpaper, false);
                    }
                }
            }

            ApplyAudioSource();
        }

        public static void CloseWallpaper(params string[] screens)
        {
            foreach (var screenItem in screens)
            {
                if (CurrentWalpapers.ContainsKey(screenItem))
                    CurrentWalpapers.Remove(screenItem);
            }
            InnerCloseWallpaper(screens);
        }

        public static Task SetOptions(LiveWallpaperOptions options)
        {
            Options = options;

            ExplorerMonitor.ExpolrerCreated -= ExplorerMonitor_ExpolrerCreated;
            MaximizedMonitor.AppMaximized -= MaximizedMonitor_AppMaximized;

            if (options.AutoRestartWhenExplorerCrash == true)
                ExplorerMonitor.ExpolrerCreated += ExplorerMonitor_ExpolrerCreated;

            bool enableMaximized = options.ScreenOptions.ToList().Exists(m => m.WhenAppMaximized != ActionWhenMaximized.Play);
            if (enableMaximized)
                MaximizedMonitor.AppMaximized += MaximizedMonitor_AppMaximized;

            StartTimer(options.AutoRestartWhenExplorerCrash || enableMaximized);

            ApplyAudioSource();

            return Task.CompletedTask;
        }

        public static void Pause(params string[] screens)
        {
            foreach (var screenItem in screens)
            {
                if (CurrentWalpapers.ContainsKey(screenItem))
                {
                    var wallpaper = CurrentWalpapers[screenItem];
                    var currentRender = RenderFactory.GetRenderByExtension(Path.GetExtension(wallpaper.WPModel.Path));
                    currentRender.Pause(screens);
                }
            }
        }

        public static void Resume(params string[] screens)
        {
            foreach (var screenItem in screens)
            {
                if (CurrentWalpapers.ContainsKey(screenItem))
                {
                    var wallpaper = CurrentWalpapers[screenItem];
                    var currentRender = RenderFactory.GetRenderByExtension(Path.GetExtension(wallpaper.WPModel.Path));
                    currentRender.Resume(screens);
                }
            }
        }

        #endregion

        #region private

        private static void ApplyAudioSource()
        {
            //设置音源
            foreach (var screen in Screens)
            {
                if (CurrentWalpapers.ContainsKey(screen))
                {
                    var wallpaper = CurrentWalpapers[screen];
                    var currentRender = RenderFactory.GetRender(wallpaper.WPModel);
                    currentRender.SetVolume(screen == Options.AudioScreen ? 100 : 0, screen);
                }
            }
        }

        private static void InnerCloseWallpaper(params string[] screens)
        {
            RenderFactory.Renders.ForEach(m => m.CloseWallpaper(screens));
        }

        private static void StartTimer(bool enable)
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
                if (_timer != null)
                {
                    _timer.Elapsed -= Timer_Elapsed;
                    _timer.Stop();
                    _timer = null;
                }
            }
        }

        #endregion

        #region callback

        private static void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            _timer?.Stop();
            ExplorerMonitor.Check();
            MaximizedMonitor.Check();
            _timer?.Start();
        }

        private static void ExplorerMonitor_ExpolrerCreated(object sender, EventArgs e)
        {
            //重启
            //Process.Start(Application.ResourceAssembly.Location);
            //Application.Current.Shutdown();
            //Application.Restart();
        }
        private static void MaximizedMonitor_AppMaximized(object sender, AppMaximizedEvent e)
        {
            var maximizedScreens = e.MaximizedScreens.Select((m, i) => m.DeviceName).ToList();
            bool anyScreenMaximized = maximizedScreens.Count > 0;
            foreach (var item in Options.ScreenOptions)
            {
                string currentScreen = item.Screen;
                bool currentScreenMaximized = maximizedScreens.Contains(currentScreen) || Options.AppMaximizedEffectAllScreen && anyScreenMaximized;

                switch (item.WhenAppMaximized)
                {
                    case ActionWhenMaximized.Pause:
                        if (currentScreenMaximized)
                            Pause(currentScreen);
                        else
                            Resume(currentScreen);
                        break;
                    case ActionWhenMaximized.Stop:
                        if (currentScreenMaximized)
                        {
                            InnerCloseWallpaper(currentScreen);
                            CurrentWalpapers[currentScreen] = (CurrentWalpapers[currentScreen].WPModel, true);

                        }
                        else if (CurrentWalpapers.ContainsKey(currentScreen))
                        {
                            _ = ShowWallpaper(CurrentWalpapers[currentScreen].WPModel, currentScreen);
                        }
                        break;
                    case ActionWhenMaximized.Play:
                        break;
                }
            }
        }

        #endregion
    }
}
