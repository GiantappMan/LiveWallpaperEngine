using DZY.WinAPI;
using Giantapp.LiveWallpaper.Engine.Renders;
using Giantapp.LiveWallpaper.Engine.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
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
        public static LiveWallpaperOptions Options { get; private set; } = new LiveWallpaperOptions();

        //Dictionary<DeviceName，WallpaperModel>
        public static Dictionary<string, WallpaperModel> CurrentWalpapers { get; private set; } = new Dictionary<string, WallpaperModel>();

        public static bool Initialized { get; private set; }

        public static List<(WallpaperType Type, string DownloadUrl)> PlayerUrls = new List<(WallpaperType Type, string DownloadUrl)>()
        {
            (WallpaperType.Video,"https://github.com/giant-app/LiveWallpaperEngine/releases/download/v2.0.4/mpv.7z"),
            (WallpaperType.Web,"https://github.com/giant-app/LiveWallpaperEngine/releases/download/v2.0.4/web.7z"),
        };

        public static event EventHandler<DownloadPlayerProgressArgs> DownloadPlayerProgressChangedEvent;
        public static event EventHandler<SetupPlayerProgressArgs> SetupPlayerProgressChangedEvent;

        #endregion

        #region public

        public static void Initlize(Dispatcher dispatcher)
        {
            RenderFactory.Renders.Add(new ExeRender());
            RenderFactory.Renders.Add(new VideoRender());
            RenderFactory.Renders.Add(new WebRender());
            RenderFactory.Renders.Add(new ImageRender());
            _uiDispatcher = dispatcher;
            Screens = Screen.AllScreens.Select(m => m.DeviceName).ToArray();
            Initialized = true;
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

        public static async Task<ShowWallpaperResult> ShowWallpaper(WallpaperModel wallpaper, params string[] screens)
        {
            if (!Initialized)
                throw new ArgumentException("You need to initialize the SDK first");

            if (screens.Length == 0)
                screens = Screens;

            IRender currentRender;
            if (wallpaper.Type == null)
            {
                currentRender = RenderFactory.GetRenderByExtension(Path.GetExtension(wallpaper.Path));
                if (currentRender == null)
                    return new ShowWallpaperResult()
                    {
                        Ok = false,
                        Error = ShowWallpaperResult.ErrorType.NoRender,
                        Message = "This wallpaper type is not supported"
                    };

                wallpaper.Type = currentRender.SupportType;
            }
            else
                currentRender = RenderFactory.GetRender(wallpaper.Type.Value);

            if (currentRender == null)
                if (wallpaper.Type == null)
                    throw new ArgumentException("Unsupported wallpaper type");

            foreach (var screenItem in screens)
            {
                //当前屏幕没有壁纸
                if (!CurrentWalpapers.ContainsKey(screenItem))
                    CurrentWalpapers.Add(screenItem, null);

                var existWallpaper = CurrentWalpapers[screenItem];

                //壁纸 路径相同
                if (existWallpaper != null && existWallpaper.Path == wallpaper.Path)
                    continue;

                //关闭之前的壁纸
                await CloseWallpaper(screenItem);
                var showResult = await currentRender.ShowWallpaper(wallpaper, screenItem);
                if (!showResult.Ok)
                    return showResult;
                CurrentWalpapers[screenItem] = wallpaper;
            }

            ApplyAudioSource();
            return new ShowWallpaperResult() { Ok = true };
        }

        public static async Task CloseWallpaper(params string[] screens)
        {
            foreach (var screenItem in screens)
            {
                if (CurrentWalpapers.ContainsKey(screenItem))
                    CurrentWalpapers.Remove(screenItem);
            }
            await InnerCloseWallpaper(screens);
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
                    wallpaper.IsPaused = true;
                    var currentRender = RenderFactory.GetRenderByExtension(Path.GetExtension(wallpaper.Path));
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
                    wallpaper.IsPaused = false;
                    var currentRender = RenderFactory.GetRenderByExtension(Path.GetExtension(wallpaper.Path));
                    currentRender.Resume(screens);
                }
            }
        }

        public static async Task SetupPlayer(WallpaperType type, string zipFile)
        {

            void ArchiveFile_UnzipProgressChanged(object sender, SevenZipUnzipProgressArgs e)
            {
                SetupPlayerProgressChangedEvent?.Invoke(null, new SetupPlayerProgressArgs()
                {
                    Completed = false,
                    FilePath = zipFile,
                    ProgressPercent = e.Progress,
                });
            }

            if (File.Exists(zipFile))
            {
                string distFolder = null;
                switch (type)
                {
                    case WallpaperType.Web:
                        distFolder = WebRender.PlayerFolderName;
                        break;
                    case WallpaperType.Video:
                        distFolder = VideoRender.PlayerFolderName;
                        break;
                }
                SevenZip archiveFile = new SevenZip(zipFile);
                archiveFile.UnzipProgressChanged += ArchiveFile_UnzipProgressChanged;
                string dist = $@"{Options.ExternalPlayerFolder}\{distFolder}";

                try
                {
                    await Task.Run(() => archiveFile.Extract(dist));
                    SetupPlayerProgressChangedEvent?.Invoke(null, new SetupPlayerProgressArgs()
                    {
                        Completed = true,
                        FilePath = zipFile,
                        ProgressPercent = 100
                    });
                }
                catch (Exception ex)
                {
                    SetupPlayerProgressChangedEvent?.Invoke(null, new SetupPlayerProgressArgs()
                    {
                        Completed = true,
                        FilePath = zipFile,
                        Error = ex.Message,
                        ProgressPercent = 100
                    });
                }
                finally
                {
                    archiveFile.UnzipProgressChanged -= ArchiveFile_UnzipProgressChanged;
                }

            }
        }

        public static async Task<string> DownloadPlayer(WallpaperType type, string url)
        {
            string downloadFile = Path.Combine(Options.ExternalPlayerFolder, $"tmp{type}");
            if (File.Exists(downloadFile))
                return downloadFile;

            void Client_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
            {
                var args = new DownloadPlayerProgressArgs()
                {
                    Completed = true,
                    DownloadUrl = url,
                    ProgressPercent = 100,
                    Error = e.Error?.ToString()
                };
                DownloadPlayerProgressChangedEvent?.Invoke(e, args);
            }
            void Client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
            {
                var args = new DownloadPlayerProgressArgs()
                {
                    Completed = false,
                    DownloadUrl = url,
                    ProgressPercent = (float)e.BytesReceived / e.TotalBytesToReceive
                };
                DownloadPlayerProgressChangedEvent?.Invoke(e, args);
            }

            var client = new WebClient();
            client.DownloadProgressChanged += Client_DownloadProgressChanged;
            client.DownloadFileCompleted += Client_DownloadFileCompleted;
            try
            {
                await client.DownloadFileTaskAsync(url, downloadFile);
                return downloadFile;
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                client.DownloadProgressChanged -= Client_DownloadProgressChanged;
                client.DownloadFileCompleted -= Client_DownloadFileCompleted;
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
                    var currentRender = RenderFactory.GetRender(wallpaper);
                    currentRender.SetVolume(screen == Options.AudioScreen ? 100 : 0, screen);
                }
            }
        }

        private static async Task InnerCloseWallpaper(params string[] screens)
        {
            foreach (var m in RenderFactory.Renders)
                await m.CloseWallpaperAsync(screens);
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
        private static async void MaximizedMonitor_AppMaximized(object sender, AppMaximizedEvent e)
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
                            await InnerCloseWallpaper(currentScreen);
                            CurrentWalpapers[currentScreen].IsStopedTemporary = true;
                        }
                        else if (CurrentWalpapers.ContainsKey(currentScreen))
                        {
                            //_ = ShowWallpaper(CurrentWalpapers[currentScreen], currentScreen);

                            var wallpaper = CurrentWalpapers[currentScreen];
                            var currentRender = RenderFactory.GetRenderByExtension(Path.GetExtension(wallpaper.Path));
                            await currentRender.ShowWallpaper(wallpaper, currentScreen);
                        }
                        break;
                    case ActionWhenMaximized.Play:
                        CurrentWalpapers[currentScreen].IsStopedTemporary = false;
                        break;
                }
            }
        }

        #endregion
    }

    public class MyWebClient : WebClient
    {
        protected override WebRequest GetWebRequest(Uri address)
        {
            var request = (HttpWebRequest)base.GetWebRequest(address);
            request.AllowAutoRedirect = true;
            return request;
        }
    }
}
