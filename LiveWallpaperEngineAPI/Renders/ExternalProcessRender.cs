using DZY.WinAPI;
using DZY.WinAPI.Desktop.API;
using Giantapp.LiveWallpaper.Engine.Forms;
using Giantapp.LiveWallpaper.Engine.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace Giantapp.LiveWallpaper.Engine.Renders
{
    //目前所有壁纸都是这个类实现，通过启用外部exe来渲染，以防止崩溃。
    public class ExternalRender : IRender
    {
        private int _currentPid = -1;
        private Dictionary<string, WallpaperModel> _currentWallpapers = new Dictionary<string, WallpaperModel>();

        public WallpaperType SupportedType { get; private set; }

        public List<string> SupportedExtension { get; private set; }

        protected ExternalRender(WallpaperType type, List<string> extension)
        {
            SupportedType = type;
            SupportedExtension = extension;
        }

        public void CloseWallpaper(params string[] screens)
        {
          
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
            throw new NotImplementedException();
        }

        public void Resum(params string[] screens)
        {
            throw new NotImplementedException();
        }

        public void SetVolume(int v, params string[] screens)
        {
            throw new NotImplementedException();
        }

        public async Task ShowWallpaper(WallpaperModel wallpaper, params string[] screens)
        {
            foreach (var screenItem in screens)
            {
                if (_currentWallpapers.ContainsKey(screenItem) && _currentWallpapers[screenItem].Path == wallpaper.Path)
                {
                    //壁纸未变
                    continue;
                }

                IntPtr windowHanlde = await GetWindowHandle(wallpaper.Path);

                //壁纸启动失败
                if (windowHanlde == IntPtr.Zero)
                    continue;

                var host = LiveWallpaperRenderForm.GetHost(screenItem);
                host!.ShowWallpaper(windowHanlde);

                _currentWallpapers[screenItem] = wallpaper;
            }

            //Stopwatch sw = new Stopwatch();
            //sw.Start();
            //int timeout = 10 * 1000;

            //ProcessStartInfo info = new ProcessStartInfo(wallpaper.Path);
            //info.WindowStyle = ProcessWindowStyle.Maximized;
            //info.CreateNoWindow = true;
            //Process targetProcess = Process.Start(info);

            //while (targetProcess.MainWindowHandle == IntPtr.Zero)
            //{
            //    System.Threading.Thread.Sleep(10);
            //    int pid = targetProcess.Id;
            //    targetProcess.Dispose();
            //    //mainWindowHandle不会变，重新获取
            //    targetProcess = Process.GetProcessById(pid);

            //    if (sw.ElapsedMilliseconds > timeout)
            //    {
            //        sw.Stop();
            //        return;
            //    }
            //}

            //_currentTargetHandle = targetProcess.MainWindowHandle;
            //_currentPid = targetProcess.Id;

            //DesktopMouseEventReciver.HTargetWindows.Add(_currentTargetHandle);

            //// 用当前窗口显示exe
            //User32Wrapper.SetParent(_currentTargetHandle, containerHandle);
            //WallpaperHelper.FullScreen(_currentTargetHandle, containerHandle);
        }

        private Task<IntPtr> GetWindowHandle(string path)
        {
            throw new NotImplementedException();
        }
    }
}
