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
        private Dictionary<string, (WallpaperModel Wallpaper, int PId)> _currentWallpapers = new Dictionary<string, (WallpaperModel Wallpaper, int PId)>();

        List<CancellationTokenSource> _ctsList = new List<CancellationTokenSource>();


        public WallpaperType SupportedType { get; private set; }

        public List<string> SupportedExtension { get; private set; }

        protected ExternalProcessRender(WallpaperType type, List<string> extension)
        {
            SupportedType = type;
            SupportedExtension = extension;
        }

        public void CloseWallpaper(params string[] screens)
        {
            //取消等待未启动的进程
            foreach (var item in _ctsList)
                item.Cancel();

            _ctsList = new List<CancellationTokenSource>();

            var playingWallpaper = _currentWallpapers.Where(m => screens.Contains(m.Key)).ToList();

            foreach (var wallpapaer in playingWallpaper)
            {
                var p = Process.GetProcessById(wallpapaer.Value.PId);
                p.Kill();

                _currentWallpapers.Remove(wallpapaer.Key);
            }
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public int GetVolume(params string[] screens)
        {
            throw new NotImplementedException();
        }

        public void Pause(params string[] screens)
        {
            var playingWallpaper = _currentWallpapers.Where(m => screens.Contains(m.Key)).ToList();

            foreach (var wallpaper in playingWallpaper)
            {
                var p = Process.GetProcessById(wallpaper.Value.PId);
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
                    var p = Process.GetProcessById(wallpaper.Value.PId);
                    p.Resume();
                }
                catch (Exception)
                {
                    continue;
                }
            }
        }

        public void SetVolume(int v, params string[] screens)
        {
            throw new NotImplementedException();
        }

        public async Task ShowWallpaper(WallpaperModel wallpaper, params string[] screens)
        {
            List<Task> tmpTasks = new List<Task>();
            foreach (var screenItem in screens)
            {
                if (_currentWallpapers.ContainsKey(screenItem) && _currentWallpapers[screenItem].Wallpaper.Path == wallpaper.Path)
                {
                    //壁纸未变
                    continue;
                }

                var cts = new CancellationTokenSource();
                _ctsList.Add(cts);
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

                           _currentWallpapers[screenItem] = (wallpaper, result.PId);
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
                           _ctsList.Remove(cts);
                       }
                   }, cts.Token);

                tmpTasks.Add(task);

                await Task.WhenAll(tmpTasks);
            }
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
