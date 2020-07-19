using DZY.WinAPI;
using DZY.WinAPI.Desktop.API;
using DZY.WinAPI.Extension;
using Giantapp.LiveWallpaper.Engine.Forms;
using Giantapp.LiveWallpaper.Engine.Utils;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Giantapp.LiveWallpaper.Engine.Renders
{
    //目前所有壁纸都是这个类实现，通过启用外部exe来渲染，以防止崩溃。
    public class ExternalProcessRender : IRender
    {
        protected Dictionary<string, (WallpaperModel Wallpaper, (IntPtr Handle, int PId) ProcessInfo)> _currentWallpapers = new Dictionary<string, (WallpaperModel Wallpaper, (IntPtr Handle, int PId) ProcessInfo)>();

        List<(CancellationTokenSource cts, string screen)> _ctsList = new List<(CancellationTokenSource cts, string screen)>();


        public WallpaperType SupportedType { get; private set; }

        public List<string> SupportedExtension { get; private set; }

        protected ExternalProcessRender(WallpaperType type, List<string> extension)
        {
            SupportedType = type;
            SupportedExtension = extension;
        }

        public virtual void CloseWallpaper(params string[] screens)
        {
            foreach (var item in screens)
                System.Diagnostics.Debug.WriteLine($"close {this.GetType().Name} {item}");
            //取消对应屏幕等待未启动的进程
            foreach (var item in _ctsList.Where(m => screens.Contains(m.screen)))
                item.cts.Cancel();

            _ctsList = new List<(CancellationTokenSource cts, string screen)>();

            var playingWallpaper = _currentWallpapers.Where(m => screens.Contains(m.Key)).ToList();

            foreach (var wallpapaer in playingWallpaper)
            {
                var p = Process.GetProcessById(wallpapaer.Value.ProcessInfo.PId);
                p.Kill();

                _currentWallpapers.Remove(wallpapaer.Key);
            }
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public int GetVolume(string screen)
        {
            var playingWallpaper = _currentWallpapers.Where(m => screen == m.Key).FirstOrDefault();
            int result = AudioHelper.GetVolume(playingWallpaper.Value.ProcessInfo.PId);
            return result;
        }

        public void Pause(params string[] screens)
        {
            var playingWallpaper = _currentWallpapers.Where(m => screens.Contains(m.Key)).ToList();

            foreach (var wallpaper in playingWallpaper)
            {
                var p = Process.GetProcessById(wallpaper.Value.ProcessInfo.PId);
                p.Suspend();
            }
        }

        public void Resume(params string[] screens)
        {
            var playingWallpaper = _currentWallpapers.Where(m => screens.Contains(m.Key)).ToList();

            foreach (var wallpaper in playingWallpaper)
            {
                try
                {
                    var p = Process.GetProcessById(wallpaper.Value.ProcessInfo.PId);
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
            var playingWallpaper = _currentWallpapers.Where(m => screen == m.Key).FirstOrDefault();
            AudioHelper.SetVolume(playingWallpaper.Value.ProcessInfo.PId, v);
        }

        public virtual async Task ShowWallpaper(WallpaperModel wallpaper, params string[] screens)
        {
            foreach (var item in screens)
                System.Diagnostics.Debug.WriteLine($"show {this.GetType().Name} {item}");

            List<Task> tmpTasks = new List<Task>();
            foreach (var screenItem in screens)
            {
                if (_currentWallpapers.ContainsKey(screenItem) && _currentWallpapers[screenItem].Wallpaper.Path == wallpaper.Path)
                {
                    //壁纸未变
                    continue;
                }

                var cts = new CancellationTokenSource();
                var ctsData = (cts, screenItem);
                _ctsList.Add(ctsData);
                var task = Task.Run(async () =>
                   {
                       try
                       {
                           var result = await StartProcess(wallpaper.Path, cts.Token);

                           //壁纸启动失败
                           if (result.Handle == IntPtr.Zero)
                               return;

                           var host = LiveWallpaperRenderForm.GetHost(screenItem);
                           host!.ShowWallpaper(result.Handle);

                           _currentWallpapers[screenItem] = (wallpaper, result);
                       }
                       catch (OperationCanceledException)
                       {

                       }
                       catch (Exception ex)
                       {
                           Debug.WriteLine(ex);
                       }
                       finally
                       {
                           _ctsList.Remove(ctsData);
                       }
                   }, cts.Token);

                tmpTasks.Add(task);
            }
            await Task.WhenAll(tmpTasks);
        }

        private Task<(IntPtr Handle, int PId)> StartProcess(string path, CancellationToken ct)
        {
            return Task.Run(() =>
           {
               Stopwatch sw = new Stopwatch();
               sw.Start();
               int timeout = 30 * 1000;

               ProcessStartInfo info = GenerateProcessInfo(path);
               if (info == null)
                   return (IntPtr.Zero, -1);
               info.WindowStyle = ProcessWindowStyle.Maximized;
               info.CreateNoWindow = true;
               Process targetProcess = Process.Start(info);         

               while (targetProcess.MainWindowHandle == IntPtr.Zero)
               {
                   if (ct.IsCancellationRequested)
                       targetProcess.Kill();
                   ct.ThrowIfCancellationRequested();
                   Thread.Sleep(1);
                   int pid = targetProcess.Id;
                   targetProcess.Dispose();
                   //mainWindowHandle不会变，重新获取
                   targetProcess = Process.GetProcessById(pid);

                   if (sw.ElapsedMilliseconds > timeout)
                   {
                       sw.Stop();
                       break;
                   }
               }

               (IntPtr Handle, int PId) result = (targetProcess.MainWindowHandle, targetProcess.Id);
               targetProcess.Dispose();
               return result;
           });
        }

        protected virtual ProcessStartInfo GenerateProcessInfo(string path)
        {
            return new ProcessStartInfo(path);
        }
    }
}
