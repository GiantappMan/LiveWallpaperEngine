using DZY.WinAPI;
using DZY.WinAPI.Desktop.API;
using System;
using System.Drawing;

namespace LiveWallpaperEngine.Common
{
    public class WallpaperHelper
    {
        #region fields

        #region static

        IntPtr _currentHandler;
        IntPtr _parentHandler;
        RECT? _originalRect;//窗口原始大小，恢复时使用
        Rectangle _targetBounds;
        static IDesktopWallpaper _desktopWallpaperAPI;
        static IntPtr _workerw = IntPtr.Zero;
        static readonly uint _slideshowTick;

        #endregion

        #endregion

        #region construct
        static WallpaperHelper()
        {
            _ = User32Wrapper.SystemParametersInfo(User32Wrapper.SPI_SETCLIENTAREAANIMATION, 0, true, User32Wrapper.SPIF_UPDATEINIFILE | User32Wrapper.SPIF_SENDWININICHANGE);
            _desktopWallpaperAPI = GetDesktopWallpaperAPI();
            _desktopWallpaperAPI?.GetSlideshowOptions(out _, out _slideshowTick);
            _desktopWallpaperAPI?.SetSlideshowOptions(DesktopSlideshowOptions.DSO_SHUFFLEIMAGES, 1000 * 60 * 60 * 24);
        }

        ~WallpaperHelper()
        {
            _desktopWallpaperAPI?.SetSlideshowOptions(DesktopSlideshowOptions.DSO_SHUFFLEIMAGES, _slideshowTick);
        }

        //禁止外部程序集直接构造
        public WallpaperHelper(Rectangle bounds)
        {
            _targetBounds = bounds;
            //_workerw = GetWorkerW();
        }

        #endregion

        #region  public methods

        public void RestoreParent()
        {
            if (_workerw == IntPtr.Zero)
                _workerw = GetWorkerW();

            User32Wrapper.SetParent(_currentHandler, _parentHandler);

            //恢复原始大小
            if (_originalRect != null)
                User32WrapperEx.SetWindowPosEx(_currentHandler, _originalRect.Value);
        }

        public bool SendToBackground(IntPtr handler)
        {
            if (handler != _currentHandler)
                //已经换了窗口，恢复上一个窗口
                RestoreParent();

            if (handler == IntPtr.Zero)
                return false;

            var ok = User32Wrapper.GetWindowRect(handler, out RECT react);
            if (ok)
                _originalRect = react;

            _currentHandler = handler;
            //if (_workerw == IntPtr.Zero)
            //{
            var _workerw = GetWorkerW();
            if (_workerw == IntPtr.Zero)
            {
                //有时候突然又不行了，在来一次
                _ = User32Wrapper.SystemParametersInfo(User32Wrapper.SPI_SETCLIENTAREAANIMATION, 0, true, User32Wrapper.SPIF_UPDATEINIFILE | User32Wrapper.SPIF_SENDWININICHANGE);
                _workerw = GetWorkerW();
                return false;
            }
            //}

            _parentHandler = User32Wrapper.GetParent(_currentHandler);
            if (_parentHandler == IntPtr.Zero)
                _parentHandler = User32Wrapper.GetAncestor(_currentHandler, GetAncestorFlags.GetParent);

            User32Wrapper.SetParent(_currentHandler, _workerw);

            FullScreen(_currentHandler, _targetBounds);
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

            var desktopWallpaperAPI = GetDesktopWallpaperAPI();
            RefreshWallpaper(desktopWallpaperAPI);
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

        private static void FullScreen(IntPtr targeHandler, Rectangle bounds)
        {
            RECT rect = new RECT(bounds);

            User32Wrapper.MapWindowPoints(IntPtr.Zero, _workerw, ref rect, 2);
            _ = User32WrapperEx.SetWindowPosEx(targeHandler, rect);
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
