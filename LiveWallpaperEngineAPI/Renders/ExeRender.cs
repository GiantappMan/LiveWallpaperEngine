using DZY.WinAPI;
using EventHook;
using LiveWallpaperEngineAPI.Common;
using LiveWallpaperEngineAPI.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LiveWallpaperEngineAPI.Renders
{
    public class ExeRender : IRender
    {
        public List<WallpaperType> SupportTypes => StaticSupportTypes;
        static List<(uint screenIndex, int pid)> _currentProcress = new List<(uint screenIndex, int pid)>();

        public static List<WallpaperType> StaticSupportTypes => new List<WallpaperType>()
        {
           WallpaperType.Exe,
        };

        public ExeRender()
        {
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public int GetVolume(params uint[] screenIndexs)
        {
            throw new NotImplementedException();
        }

        public void Pause(params uint[] screenIndexs)
        {
        }

        public void Resum(params uint[] screenIndexs)
        {
        }

        public void SetVolume(int v, params uint[] screenIndexs)
        {

        }

        public async Task ShowWallpaper(WallpaperModel wallpaper, params uint[] screenIndexs)
        {
            try
            {
                foreach (var index in screenIndexs)
                {
                    var process = await Task.Run(() =>
                    {
                        ProcessStartInfo startInfo = new ProcessStartInfo(wallpaper.Path);                        
                        //startInfo.WindowStyle = ProcessWindowStyle.Maximized;
                        //startInfo.Arguments = "-window-mode Windowed";
                        var p = Process.Start(startInfo);
                        //等待窗口句柄创建完成
                        while (p.MainWindowHandle == IntPtr.Zero)
                        {
                            int pid = p.Id;
                            p.Dispose();
                            //mainWindowHandle不会变，重新获取
                            p = Process.GetProcessById(pid);
                            Thread.Sleep(1000);
                        }
                        //MoveWindow(p.MainWindowHandle, -1000, -1000, 500, 500, true);
                        return p;
                    });
                    //_ = User32WrapperEx.SetWindowPosEx(process.MainWindowHandle, new RECT(-5000, -5000, 1, 1));

                    _currentProcress.Add((index, process.Id));
                    WallpaperHelper.GetInstance(index).SendToBackground(process.MainWindowHandle);
                    // 设置要接受鼠标消息的窗口的句柄
                    DesktopMouseEventReciver.HTargetWindows.Add(process.MainWindowHandle);
                    process.Dispose();
                }
                await Task.Run(DesktopMouseEventReciver.Start);
            }
            catch (Exception ex)
            {

            }
        }
        [System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true)]
        internal static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);
        public void CloseWallpaper(params uint[] screenIndexs)
        {
            var tmpList = _currentProcress.ToList();
            foreach (var item in tmpList)
            {
                if (screenIndexs.ToList().Contains(item.screenIndex))
                {
                    var p = Process.GetProcessById(item.pid);
                    p.Kill();

                    _currentProcress.Remove(item);
                }
            }

            var haveExeWallpaper = WallpaperManager.Instance.CurrentWalpapers.Values.FirstOrDefault(m => m.Type == WallpaperType.Exe) != null;
            if (!haveExeWallpaper)
                DesktopMouseEventReciver.Stop();
        }
    }
}
