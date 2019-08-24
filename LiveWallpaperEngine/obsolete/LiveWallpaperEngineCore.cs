//原理参考
//https://www.codeproject.com/Articles/856020/Draw-Behind-Desktop-Icons-in-Windows 
//https://github.com/Francesco149/weebp/blob/master/src/weebp.c 
using DZY.WinAPI;
using DZY.WinAPI.Desktop.API;
using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace LiveWallpaperEngine
{
    /// <summary>
    /// 管理一个显示器的壁纸
    /// </summary>
    public class LiveWallpaperEngineCore
    {
        #region fields

        #region static

        IntPtr _currentHandler;
        IntPtr _parentHandler;
        RECT? _originalRect;//窗口原始大小，恢复时使用

        static IDesktopWallpaper _desktopWallpaperAPI;
        static IntPtr _workerw = IntPtr.Zero;
        static uint _slideshowTick;

        #endregion

        //公开属性
        public bool Shown { get; private set; }
        public Screen DisplayScreen { get; private set; }

        #endregion

        #region construct

        //禁止外部程序集直接构造
        internal LiveWallpaperEngineCore(Screen screen)
        {
            DisplayScreen = screen;
            ReInit();
        }

        internal void ReInit()
        {
            _workerw = GetWorkerW();
            //explore重启后，之前的窗口已经挂了不能恢复
            Shown = false;

            var _desktopWallpaperAPI = GetDesktopWallpaperAPI();
            _desktopWallpaperAPI?.GetSlideshowOptions(out DesktopSlideshowOptions temp, out _slideshowTick);
            _desktopWallpaperAPI?.SetSlideshowOptions(DesktopSlideshowOptions.DSO_SHUFFLEIMAGES, 1000 * 60 * 60 * 24);
        }

        #endregion

        #region  public methods

        public void RestoreParent(bool refreshWallpaper = true)
        {
            if (!Shown)
                return;

            if (refreshWallpaper)
                _desktopWallpaperAPI = RefreshWallpaper(_desktopWallpaperAPI);

            if (_workerw == IntPtr.Zero)
                _workerw = GetWorkerW();

            User32Wrapper.SetParent(_currentHandler, _parentHandler);

            if (_originalRect != null)
                User32WrapperEx.SetWindowPosEx(_currentHandler, _originalRect.Value);
            Shown = false;
        }

        public void Close()
        {
            Shown = false;
        }

        public bool SendToBackground(IntPtr handler)
        {
            if (Shown && handler != _currentHandler)
            {
                //已经换了窗口，恢复上一个窗口
                RestoreParent(false);
            }

            if (handler == IntPtr.Zero || Shown)
                return false;

            var ok = User32Wrapper.GetWindowRect(handler, out RECT react);
            if (ok)
                _originalRect = react;

            Shown = true;
            _currentHandler = handler;

            if (_workerw == IntPtr.Zero)
            {
                _workerw = GetWorkerW();
                if (_workerw == IntPtr.Zero)
                    return false;
            }

            _parentHandler = User32Wrapper.GetParent(_currentHandler);
            if (_parentHandler == IntPtr.Zero)
                _parentHandler = User32Wrapper.GetAncestor(_currentHandler, GetAncestorFlags.GetParent);

            User32Wrapper.SetParent(_currentHandler, _workerw);

            FullScreen(_currentHandler, DisplayScreen);

            return true;
        }

        /// <summary>
        /// 恢复WorkerW中的所有句柄到桌面
        /// </summary>
        public static void RestoreAllHandles()
        {
            var desktop = User32Wrapper.GetDesktopWindow();
            var workw = GetWorkerW();
            var enumWindowResult = User32Wrapper.EnumChildWindows(workw, new EnumWindowsProc((tophandle, topparamhandle) =>
            {
                var txt = User32WrapperEx.GetWindowTextEx(tophandle);
                if (!string.IsNullOrEmpty(txt))
                {
                    User32Wrapper.SetParent(tophandle, desktop);
                }

                return true;
            }), IntPtr.Zero);

            RefreshWallpaper(null);
        }

        public static void Dispose()
        {
            _desktopWallpaperAPI?.SetSlideshowOptions(DesktopSlideshowOptions.DSO_SHUFFLEIMAGES, _slideshowTick);
        }

        public static IDesktopWallpaper GetDesktopWallpaperAPI()
        {
            try
            {
                var result = DesktopWallpaperFactory.Create();
                return result;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                return null;
            }
        }

        #endregion

        #region private
        internal static IntPtr GetWorkerW()
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

        private static void FullScreen(IntPtr targeHandler, Screen displayScreen)
        {
            RECT rect = new RECT(displayScreen.Bounds);

            User32Wrapper.MapWindowPoints(IntPtr.Zero, _workerw, ref rect, 2);
            var ok = User32WrapperEx.SetWindowPosEx(targeHandler, rect);
            return;
        }

        //刷新壁纸
        private static IDesktopWallpaper RefreshWallpaper(IDesktopWallpaper desktopWallpaperAPI)
        {
            var explorer = ExplorerMonitor.ExploreProcess;
            if (explorer == null)
                return null;

            if (desktopWallpaperAPI == null)
                desktopWallpaperAPI = GetDesktopWallpaperAPI();

            try
            {
                desktopWallpaperAPI.Enable(false);
                desktopWallpaperAPI.Enable(true);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                desktopWallpaperAPI = GetDesktopWallpaperAPI();
            }
            return desktopWallpaperAPI;
        }

        #endregion
    }
}
