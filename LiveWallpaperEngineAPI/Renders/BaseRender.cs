using DZY.WinAPI.Extension;
using Giantapp.LiveWallpaper.Engine.Forms;
using Giantapp.LiveWallpaper.Engine.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Giantapp.LiveWallpaper.Engine.Renders
{
    public class BaseRender : IRender
    {
        private readonly List<RenderInfo> _currentWallpapers = new List<RenderInfo>();

        private CancellationTokenSource _showWallpaperCts = new CancellationTokenSource();

        public WallpaperType SupportedType { get; private set; }

        public List<string> SupportedExtension { get; private set; }

        protected BaseRender(WallpaperType type, List<string> extension)
        {
            SupportedType = type;
            SupportedExtension = extension;
        }

        public async Task CloseWallpaperAsync(params string[] screens)
        {
            foreach (var item in screens)
                Debug.WriteLine($"close {GetType().Name} {item}");
            //取消对应屏幕等待未启动的进程            
            _showWallpaperCts?.Cancel();
            _showWallpaperCts?.Dispose();
            _showWallpaperCts = null;

            var playingWallpaper = _currentWallpapers.Where(m => screens.Contains(m.Screen)).ToList();

            await InnerCloseWallpaperAsync(playingWallpaper);

            var eventWallpapers = _currentWallpapers.Where(m => m.Wallpaper.IsEventWallpaper).Count();
            if (eventWallpapers == 0)
                DesktopMouseEventReciver.Stop();
        }

        private async Task InnerCloseWallpaperAsync(List<RenderInfo> playingWallpaper)
        {
            await CloseRender(playingWallpaper);

            playingWallpaper.ToList().ForEach(m =>
            {
                DesktopMouseEventReciver.RemoveHandle(m.ReceiveMouseEventHandle);
                _currentWallpapers.Remove(m);
            });
        }

        /// <summary>
        /// 可重载，处理具体的关闭逻辑
        /// </summary>
        /// <param name="playingWallpaper"></param>
        /// <param name="isTemporary">是否是临时关闭，临时关闭表示马上又会继续播放其他壁纸</param>
        /// <returns></returns>
        protected virtual Task CloseRender(List<RenderInfo> playingWallpaper, bool isTemporary = false)
        {
            return Task.CompletedTask;
        }

        public void Dispose()
        {
        }

        public int GetVolume(string screen)
        {
            var playingWallpaper = _currentWallpapers.Where(m => screen == m.Screen).FirstOrDefault();
            int result = AudioHelper.GetVolume(playingWallpaper.PId);
            return result;
        }

        public void Pause(params string[] screens)
        {
            var playingWallpaper = _currentWallpapers.Where(m => screens.Contains(m.Screen)).ToList();

            foreach (var wallpaper in playingWallpaper)
            {
                try
                {
                    var p = Process.GetProcessById(wallpaper.PId);
                    p.Suspend();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex);
                    continue;
                }
            }
        }

        public void Resume(params string[] screens)
        {
            var playingWallpaper = _currentWallpapers.Where(m => screens.Contains(m.Screen)).ToList();

            foreach (var wallpaper in playingWallpaper)
            {
                try
                {
                    var p = Process.GetProcessById(wallpaper.PId);
                    p.Resume();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex);
                    continue;
                }
            }
        }

        public void SetVolume(int v, string screen)
        {
            var playingWallpaper = _currentWallpapers.Where(m => screen == m.Screen).FirstOrDefault();
            AudioHelper.SetVolume(playingWallpaper.PId, v);
        }

        public async Task ShowWallpaper(WallpaperModel wallpaper, params string[] screens)
        {
            foreach (var item in screens)
                Debug.WriteLine($"show {GetType().Name} {item}");

            List<RenderInfo> changedRender = new List<RenderInfo>();
            //过滤无变化的屏幕
            var changedScreen = screens.Where(m =>
            {
                bool ok = false;
                var existRender = _currentWallpapers.FirstOrDefault(x => x.Screen == m);
                if (existRender == null)
                    ok = true;
                else
                {
                    ok = existRender.Wallpaper.Path != wallpaper.Path;
                    changedRender.Add(existRender);
                }
                return ok;
            }).ToArray();


            if (changedScreen.Length == 0)
                return;

            //关闭已经展现的壁纸
            await InnerCloseWallpaperAsync(changedRender);

            _showWallpaperCts = new CancellationTokenSource();
            List<RenderInfo> infos = await InnerShowWallpaper(wallpaper, _showWallpaperCts.Token, changedScreen);

            //更新当前壁纸
            infos.ForEach(m => _currentWallpapers.Add(m));

            if (WallpaperManager.Options.ForwardMouseEvent)
            {
                var eventWallpapers = _currentWallpapers.Where(m => m.Wallpaper.IsEventWallpaper).ToList();
                foreach (var item in eventWallpapers)
                    DesktopMouseEventReciver.AddHandle(item.ReceiveMouseEventHandle, item.Screen);
                if (eventWallpapers.Count > 0)
                    await Task.Run(DesktopMouseEventReciver.Start);
            }
        }

        protected virtual Task<List<RenderInfo>> InnerShowWallpaper(WallpaperModel wallpaper, CancellationToken ct, params string[] screens)
        {
            return Task.FromResult(screens.Select(m => new RenderInfo()
            {
                Wallpaper = wallpaper,
                Screen = m
            }).ToList());
        }
    }
}
