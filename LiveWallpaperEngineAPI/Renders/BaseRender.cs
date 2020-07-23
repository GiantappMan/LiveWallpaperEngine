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
    public class RenderProcess
    {
        public IntPtr HostHandle { get; set; }
        public IntPtr ReceiveMouseEventHandle { get; set; }
        public int PId { get; set; }
    }
    public class RenderInfo : RenderProcess
    {
        public RenderInfo()
        {

        }
        public RenderInfo(RenderProcess p)
        {
            HostHandle = p.HostHandle;
            ReceiveMouseEventHandle = p.ReceiveMouseEventHandle;
            PId = p.PId;
        }
        public WallpaperModel Wallpaper { get; set; }
        public string Screen { get; set; }
    }

    public class BaseRender : IRender
    {
        //screen,renderInfo
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

            await InnerCloseWallpaper(playingWallpaper);

            playingWallpaper.ForEach(m =>
            {
                DesktopMouseEventReciver.RemoveHandle(m.ReceiveMouseEventHandle);
                _currentWallpapers.Remove(m);
            });

            var eventWallpapers = _currentWallpapers.Where(m => m.Wallpaper.IsEventWallpaper).Count();
            if (eventWallpapers == 0)
                DesktopMouseEventReciver.Stop();
        }

        /// <summary>
        /// 可重载，处理具体的关闭逻辑
        /// </summary>
        /// <param name="playingWallpaper"></param>
        /// <returns></returns>
        protected virtual Task InnerCloseWallpaper(List<RenderInfo> playingWallpaper)
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
                var p = Process.GetProcessById(wallpaper.PId);
                p.Suspend();
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
                catch (Exception)
                {
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

            //过滤无变化的屏幕
            var changedScreen = screens.Where(m =>
            {
                var existRender = _currentWallpapers.FirstOrDefault(x => x.Screen == m);
                if (existRender == null)
                    return true;

                return existRender.Wallpaper.Path != wallpaper.Path;
            }).ToArray();

            _showWallpaperCts = new CancellationTokenSource();
            List<RenderInfo> infos = await InnerShowWallpaper(wallpaper, _showWallpaperCts.Token, changedScreen);

            //更新当前壁纸
            infos.ForEach(m => _currentWallpapers.Add(m));

            var eventWallpapers = _currentWallpapers.Where(m => m.Wallpaper.IsEventWallpaper).ToList();
            foreach (var item in eventWallpapers)
                DesktopMouseEventReciver.AddHandle(item.ReceiveMouseEventHandle);
            if (eventWallpapers.Count > 0)
                await Task.Run(DesktopMouseEventReciver.Start);
        }

        protected virtual Task<List<RenderInfo>> InnerShowWallpaper(WallpaperModel wallpaper, CancellationToken ct, params string[] screens)
        {
            return Task.FromResult(new List<RenderInfo>());
        }

        protected virtual ProcessStartInfo GenerateProcessInfo(string path)
        {
            return new ProcessStartInfo(path);
        }
    }
}
