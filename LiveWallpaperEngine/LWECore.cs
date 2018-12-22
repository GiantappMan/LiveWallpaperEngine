//https://www.codeproject.com/Articles/856020/Draw-Behind-Desktop-Icons-in-Windows 动态壁纸核心逻辑，就这么两句。。没什么好藏在噎着的。代码我也是从互联网抄写的-。-
//https://github.com/Francesco149/weebp/blob/master/src/weebp.c 从时间上来说这个仓库比我后写，但是却有值得学习的地方。毕竟是c语言大佬。。。
using DZY.WinAPI;
using DZY.WinAPI.Desktop.API;
using System;
using System.Threading.Tasks;

namespace LiveWallpaperEngine
{
    /// <summary>
    /// 动态壁纸最最最最最核心的逻辑，请大胆食用
    /// </summary>
    public class LWECore : IDisposable
    {
        #region fields
        IntPtr _workerw = IntPtr.Zero;
        IntPtr _targeHandler;
        IntPtr _parentHandler;
        IDesktopWallpaper _desktopWallpaperAPI;
        RECT? _originalRect;
        uint _slideshowTick;

        public bool Shown { get; private set; }
        public object Screen { get; private set; }

        #endregion

        #region construct

        public LWECore()
        {
            _desktopWallpaperAPI = DesktopWallpaperFactory.Create();
            _desktopWallpaperAPI.GetSlideshowOptions(out DesktopSlideshowOptions temp, out _slideshowTick);
            _desktopWallpaperAPI.SetSlideshowOptions(DesktopSlideshowOptions.DSO_SHUFFLEIMAGES, 1000 * 60 * 60 * 24);
        }

        public void Dispose()
        {
            _desktopWallpaperAPI.SetSlideshowOptions(DesktopSlideshowOptions.DSO_SHUFFLEIMAGES, _slideshowTick);
        }

        #endregion

        #region  public methods

        /// <summary>
        /// 恢复WorkerW中的所有句柄到桌面
        /// </summary>
        public static void RestoreAllHandles()
        {
            var desktop = User32Wrapper.GetDesktopWindow();
            var workw = GetWorkerW();
            var enumWindowResult = User32Wrapper.EnumChildWindows(workw, new EnumWindowsProc((tophandle, topparamhandle) =>
             {

                 var txt = User32Wrapper.GetWindowText(tophandle);
                 if (!string.IsNullOrEmpty(txt))
                 {
                     User32Wrapper.SetParent(tophandle, desktop);
                 }

                 return true;
             }), IntPtr.Zero);

            try
            {
                var desktopWallpaperAPI = DesktopWallpaperFactory.Create();

                //刷新壁纸
                desktopWallpaperAPI.Enable(false);
                desktopWallpaperAPI.Enable(true);
            }
            catch (Exception)
            {
            }
        }

        public static IntPtr GetWorkerW()
        {
            IntPtr progman = User32Wrapper.FindWindow("Progman", null);
            User32Wrapper.SendMessageTimeout(progman,
                                   0x052C,
                                   new IntPtr(0),
                                   IntPtr.Zero,
                                   SendMessageTimeoutFlags.SMTO_NORMAL,
                                   1000,
                                   out IntPtr unusefulResult);
            IntPtr workerw = IntPtr.Zero;
            var enumWindowResult = User32Wrapper.EnumWindows(new EnumWindowsProc((tophandle, topparamhandle) =>
            {
                IntPtr p = User32Wrapper.FindWindowEx(tophandle,
                                            IntPtr.Zero,
                                            "SHELLDLL_DefView",
                                            IntPtr.Zero);

                if (p != IntPtr.Zero)
                {
                    workerw = User32Wrapper.FindWindowEx(IntPtr.Zero,
                                             tophandle,
                                             "WorkerW",
                                             IntPtr.Zero);
                    return false;
                }

                return true;
            }), IntPtr.Zero);
            return workerw;
        }

        public void RestoreParent()
        {
            if (!Shown)
                return;

            //刷新壁纸
            _desktopWallpaperAPI.Enable(false);
            _desktopWallpaperAPI.Enable(true);

            if (_workerw == IntPtr.Zero)
                _workerw = GetWorkerW();

            User32Wrapper.SetParent(_targeHandler, _parentHandler);

            if (_originalRect != null)
                User32Wrapper.SetWindowPos(_targeHandler, _originalRect.Value);
            Shown = false;
        }

        public bool SendToBackground(IntPtr handler, bool fullScreen = true, int displayIndex = 0)
        {
            if (handler == IntPtr.Zero || Shown)
                return false;

            var ok = User32Wrapper.GetWindowRect(handler, out RECT react);
            if (ok)
                _originalRect = react;

            Shown = true;
            _targeHandler = handler;

            if (_workerw == IntPtr.Zero)
                _workerw = GetWorkerW();

            _parentHandler = User32Wrapper.GetParent(_targeHandler);
            if (_parentHandler == IntPtr.Zero)
                _parentHandler = User32Wrapper.GetAncestor(_targeHandler, GetAncestorFlags.GetParent);

            User32Wrapper.SetParent(_targeHandler, _workerw);

            if (fullScreen)
            {
                FullScreen(_targeHandler, displayIndex);
            }

            return true;
        }
        #endregion

        #region private

        private void FullScreen(IntPtr targeHandler, int displayIndex = 0)
        {
            //var tmp = User32Wrapper.MonitorFromWindow(targeHandler, User32Wrapper.MONITOR_DEFAULTTONEAREST);
            //MONITORINFO info = new MONITORINFO();

            //bool ok = User32Wrapper.GetMonitorInfo(tmp, info);
            //if (!ok)
            //    return null;

            //ok = User32Wrapper.GetWindowRect(_targeHandler, out RECT react);

            //ok = User32Wrapper.SetWindowPos(targeHandler, info.rcMonitor);
            //return react;

            var displays = User32Wrapper.GetDisplays();
            if (displays == null)
                return;
            var display = displays[displayIndex];
            User32Wrapper.MapWindowPoints(IntPtr.Zero, _workerw, ref display.rcMonitor, 2);
            var ok = User32Wrapper.SetWindowPos(targeHandler, display.rcMonitor);
            return;
        }

        #endregion
    }
}
