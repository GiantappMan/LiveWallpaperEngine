using DZY.Util.Common.Helpers;
using LiveWallpaperEngineAPI.Common;
using LiveWallpaperEngineAPI.Models;
using LiveWallpaperEngineAPI.Renders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;

namespace LiveWallpaperEngineAPI
{
    public class WallpaperManager
    {
        private static System.Timers.Timer _timer;
        private static readonly Dictionary<uint, WallpaperModel> _currentWalpapers = new Dictionary<uint, WallpaperModel>();

        private WallpaperManager()
        {

        }
        static WallpaperManager()
        {
            //注册render
            RenderFactory.Renders.Add(typeof(VideoRender), VideoRender.StaticSupportTypes);
            //RenderFactory.Renders.Add(typeof(WebRender), WebRender.StaticSupportTypes);
            RenderFactory.Renders.Add(typeof(ExeRender), ExeRender.StaticSupportTypes);
        }
        public static LiveWallpaperOptions Options { get; private set; }
        public static WallpaperManager Instance { get; private set; } = new WallpaperManager();

        public void Pause(params uint[] screenIndexs)
        {
            foreach (var index in screenIndexs)
            {
                if (_currentWalpapers.ContainsKey(index))
                {
                    var wallpaper = _currentWalpapers[index];
                    var currentRender = RenderFactory.GetOrCreateRender(wallpaper.Type);
                    currentRender.Pause(screenIndexs);
                }
            }
        }

        public void Resum(params uint[] screenIndexs)
        {
            foreach (var index in screenIndexs)
            {
                if (_currentWalpapers.ContainsKey(index))
                {
                    var wallpaper = _currentWalpapers[index];
                    var currentRender = RenderFactory.GetOrCreateRender(wallpaper.Type);
                    currentRender.Resum(screenIndexs);
                }
            }
        }

        public void Dispose()
        {
            var screenIndexs = Screen.AllScreens.Select((m, i) => (uint)i).ToArray();
            CloseWallpaper(screenIndexs);
        }
        public static IEnumerable<WallpaperModel> GetWallpapers(string dir)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(dir);
            foreach (var item in dirInfo.EnumerateFiles("project.json", SearchOption.AllDirectories).OrderByDescending(m => m.CreationTime))
            {
                var info = JsonHelper.JsonDeserializeFromFileAsync<WallpaperInfo>(item.FullName).Result;
                var wallpaparDir = Path.GetDirectoryName(item.FullName);
                yield return new WallpaperModel()
                {
                    Path = Path.Combine(wallpaparDir, info.File),
                    Info = info
                };
            }
        }
        public static async Task<bool> Delete(string absolutePath)
        {
            string dir = Path.GetDirectoryName(absolutePath);
            for (int i = 0; i < 3; i++)
            {
                await Task.Delay(1000);
                try
                {
                    //尝试删除3次 
                    Directory.Delete(dir, true);
                    return true;
                }
                catch (Exception)
                {
                }
            }
            return false;
        }
        public static string NormalizePath(string path)
        {
            if (string.IsNullOrEmpty(path))
                return null;
            var result = Path.GetFullPath(new Uri(path).LocalPath)
                       .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
                       .ToUpperInvariant();
            return result;
        }
        public static async Task<WallpaperModel> EditLocalPack(string sourceFile, string previewPath, WallpaperInfo info, string destDir)
        {
            string oldInfoPath = Path.Combine(destDir, "project.json");
            var oldInfo = await JsonHelper.JsonDeserializeFromFileAsync<WallpaperInfo>(oldInfoPath);

            string oldFile = null, oldPreview = null;
            if (oldInfo != null)
            {
                oldFile = Path.Combine(destDir, oldInfo.File);
                if (oldInfo.Preview != null)
                    oldPreview = Path.Combine(destDir, oldInfo.Preview ?? "");
            }

            if (NormalizePath(sourceFile) != NormalizePath(oldFile))
            {
                await Delete(oldFile);
                await Task.Run(() =>
                {
                    IOHelper.CopyFileToDir(sourceFile, destDir);
                });
            }
            info.Preview = Path.GetFileName(previewPath);
            if (NormalizePath(oldPreview) != NormalizePath(previewPath))
            {
                await Task.Run(() =>
                {
                    IOHelper.CopyFileToDir(previewPath, destDir);
                });
            }

            await WriteInfo(info, destDir);
            return new WallpaperModel()
            {
                Path = Path.Combine(destDir, info.File),
                Info = info
            };
        }
        public static async Task<WallpaperModel> CreateLocalPack(string sourceFile, string previewPath, WallpaperInfo info, string destDir)
        {
            string oldInfoPath = Path.Combine(sourceFile, "project.json");
            if (File.Exists(oldInfoPath))
            {
                var existInfo = await JsonHelper.JsonDeserializeFromFileAsync<WallpaperInfo>(oldInfoPath);
                info.Description ??= existInfo.Description;
                info.File ??= existInfo.File;
                info.Preview ??= existInfo.Preview;
                info.Tags ??= existInfo.Tags;
                info.Title ??= existInfo.Title;
                info.Type ??= existInfo.Type;
                info.Visibility ??= existInfo.Visibility;
                await Task.Run(() =>
                {
                    IOHelper.CopyFolder(sourceFile, destDir);
                });
            }
            else
            {
                await Task.Run(() =>
                {
                    IOHelper.CopyFileToDir(sourceFile, destDir);
                    info.Preview = Path.GetFileName(previewPath);
                    IOHelper.CopyFileToDir(previewPath, destDir);
                });
            }

            await WriteInfo(info, destDir);
            return new WallpaperModel()
            {
                Path = Path.Combine(destDir, info.File),
                Info = info
            };
        }
        private static async Task WriteInfo(WallpaperInfo wallpaperInfo, string destDir)
        {
            string destInfoPath = Path.Combine(destDir, "project.json");
            await JsonHelper.JsonSerializeAsync(wallpaperInfo, destInfoPath);
        }
        public async Task ShowWallpaper(WallpaperModel wallpaper, params uint[] screenIndexs)
        {
            if (wallpaper.Type == WallpaperType.NotSupport)
                wallpaper.Type = RenderFactory.ResoveType(wallpaper.Path);

            if (wallpaper.Type == WallpaperType.NotSupport)
                return;

            foreach (var index in screenIndexs)
            {
                //类型不一致关闭上次显示的壁纸
                if (_currentWalpapers.ContainsKey(index) && wallpaper.Type != _currentWalpapers[index].Type)
                    CloseWallpaper(screenIndexs);
            }

            var currentRender = RenderFactory.GetOrCreateRender(wallpaper.Type);
            await currentRender.ShowWallpaper(wallpaper, screenIndexs);

            foreach (var index in screenIndexs)
            {
                if (!_currentWalpapers.ContainsKey(index))
                    _currentWalpapers.Add(index, wallpaper);
                else
                    _currentWalpapers[index] = wallpaper;
            }

            ApplyAudioSource();
        }

        public void CloseWallpaper(params uint[] screenIndexs)
        {
            foreach (var index in screenIndexs)
            {
                if (_currentWalpapers.ContainsKey(index))
                    _currentWalpapers.Remove(index);
            }
            InnerCloseWallpaper(screenIndexs);
        }

        public void InnerCloseWallpaper(params uint[] screenIndexs)
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

            bool enableMaximized = options.ScreenOptions.ToList().Exists(m => m.WhenAppMaximized != ActionWhenMaximized.Play);
            if (enableMaximized)
                MaximizedMonitor.AppMaximized += MaximizedMonitor_AppMaximized;

            StartTimer(options.AutoRestartWhenExplorerCrash || enableMaximized);
            ApplyAudioSource();

            return Task.CompletedTask;
        }


        private void ApplyAudioSource()
        {
            //设置音源
            for (uint screenIndex = 0; screenIndex < Screen.AllScreens.Length; screenIndex++)
            {
                if (_currentWalpapers.ContainsKey(screenIndex))
                {
                    var wallpaper = _currentWalpapers[screenIndex];
                    var currentRender = RenderFactory.GetOrCreateRender(wallpaper.Type);
                    currentRender.SetVolume(screenIndex == Options.AudioScreenIndex ? 100 : 0, screenIndex);
                }
            }
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
            var maximizedScreenIndexs = e.MaximizedScreens.Select((m, i) => (uint)i).ToList();
            bool anyScreenMaximized = maximizedScreenIndexs.Count > 0;
            foreach (var item in Options.ScreenOptions)
            {
                uint currentScreenIndex = (uint)item.ScreenIndex;
                bool currentScreenMaximized = maximizedScreenIndexs.Contains(currentScreenIndex) || Options.AppMaximizedEffectAllScreen && anyScreenMaximized;

                switch (item.WhenAppMaximized)
                {
                    case ActionWhenMaximized.Pause:
                        if (currentScreenMaximized)
                            Pause(currentScreenIndex);
                        else
                            Resum(currentScreenIndex);
                        break;
                    case ActionWhenMaximized.Stop:
                        if (currentScreenMaximized)
                            InnerCloseWallpaper(currentScreenIndex);
                        else
                            if (_currentWalpapers.ContainsKey(currentScreenIndex))
                            _ = ShowWallpaper(_currentWalpapers[currentScreenIndex], currentScreenIndex);
                        break;
                    case ActionWhenMaximized.Play:
                        break;
                }
            }
        }
    }
}
