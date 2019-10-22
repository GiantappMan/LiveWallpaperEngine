//原理参考
//https://www.codeproject.com/Articles/856020/Draw-Behind-Desktop-Icons-in-Windows 
//https://github.com/Francesco149/weebp/blob/master/src/weebp.c 
using DZY.WinAPI;
using DZY.WinAPI.Desktop.API;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace LiveWallpaperEngineAPI.Common
{
    public class WallpaperHelper
    {
        #region fields

        IntPtr _currentHandler;
        IntPtr? _parentHandler;
        //RECT? _originalRect;//窗口原始大小，恢复时使用
        Rectangle _targetBounds;

        #region static

        static readonly Dictionary<uint, WallpaperHelper> _cacheInstances = new Dictionary<uint, WallpaperHelper>();
        static IDesktopWallpaper _desktopWallpaperAPI;
        //static IntPtr _workerw = IntPtr.Zero;
        static uint _slideshowTick;

        #endregion

        #endregion

        #region construct
        static WallpaperHelper()
        {

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
            if (_parentHandler != null)
                User32Wrapper.SetParent(_currentHandler, _parentHandler.Value);

            _parentHandler = null;

            ////恢复原始大小
            //if (_originalRect != null)
            //    User32WrapperEx.SetWindowPosEx(_currentHandler, _originalRect.Value);
        }

        public bool SendToBackground(IntPtr handler)
        {
            //处理alt+tab可以看见本程序
            //https://stackoverflow.com/questions/357076/best-way-to-hide-a-window-from-the-alt-tab-program-switcher
            int exStyle = User32Wrapper.GetWindowLong(handler, WindowLongFlags.GWL_EXSTYLE);
            exStyle |= (int)WindowStyles.WS_EX_TOOLWINDOW;
            User32Wrapper.SetWindowLong(handler, WindowLongFlags.GWL_EXSTYLE, exStyle);

            if (handler != _currentHandler)
                //已经换了窗口，恢复上一个窗口
                RestoreParent();

            if (handler == IntPtr.Zero)
                return false;

            _ = User32Wrapper.GetWindowRect(handler, out _);
            //var ok = User32Wrapper.GetWindowRect(handler, out RECT react);
            //if (ok)
            //    _originalRect = react;

            _currentHandler = handler;

            var workerw = GetWorkerW();
            if (workerw == IntPtr.Zero)
            {
                //有时候突然又不行了，在来一次
                User32Wrapper.SystemParametersInfo(User32Wrapper.SPI_SETCLIENTAREAANIMATION, 0, true, User32Wrapper.SPIF_UPDATEINIFILE | User32Wrapper.SPIF_SENDWININICHANGE);
                workerw = GetWorkerW();
            }

            if (workerw == IntPtr.Zero)
                return false;

            _parentHandler = User32Wrapper.GetParent(_currentHandler);

            //if (newParentHandler != _parentHandler)
            //{
            //    //parent没变时不重复调用，有时候会导致不可见
            User32Wrapper.SetParent(_currentHandler, workerw);
            FullScreen(_currentHandler, _targetBounds, workerw);
            //_parentHandler = newParentHandler;
            //}

            //if (_parentHandler == IntPtr.Zero)
            //    _parentHandler = User32Wrapper.GetAncestor(_currentHandler, GetAncestorFlags.GetParent);

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

        /// <summary>
        /// 获取指定屏幕的实例
        /// </summary>
        /// <param name="screenIndex"></param>
        /// <returns></returns>
        public static WallpaperHelper GetInstance(uint screenIndex)
        {
            if (!_cacheInstances.ContainsKey(screenIndex))
            {
                var bounds = Screen.AllScreens[screenIndex].Bounds;
                _cacheInstances.Add(screenIndex, new WallpaperHelper(bounds));
            }
            return _cacheInstances[screenIndex];
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

        public static void DoSomeMagic()
        {
            _ = User32Wrapper.SystemParametersInfo(User32Wrapper.SPI_SETCLIENTAREAANIMATION, 0, true, User32Wrapper.SPIF_UPDATEINIFILE | User32Wrapper.SPIF_SENDWININICHANGE);
            _desktopWallpaperAPI = GetDesktopWallpaperAPI();
            _desktopWallpaperAPI?.GetSlideshowOptions(out _, out _slideshowTick);
            if (_slideshowTick < 86400000)
                _desktopWallpaperAPI?.SetSlideshowOptions(DesktopSlideshowOptions.DSO_SHUFFLEIMAGES, 1000 * 60 * 60 * 24);
        }

        public static void RestoreMagic()
        {
            _desktopWallpaperAPI?.SetSlideshowOptions(DesktopSlideshowOptions.DSO_SHUFFLEIMAGES, _slideshowTick);
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

        private static void FullScreen(IntPtr targeHandler, Rectangle bounds,IntPtr workerw)
        {
            RECT rect = new RECT(bounds);

            User32Wrapper.MapWindowPoints(IntPtr.Zero, workerw, ref rect, 2);
            _ = User32WrapperEx.SetWindowPosEx(targeHandler, rect);
        }

        //刷新壁纸
        public static IDesktopWallpaper RefreshWallpaper(IDesktopWallpaper desktopWallpaperAPI = null)
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
            }
            return desktopWallpaperAPI;
        }

        #endregion
    }
}
