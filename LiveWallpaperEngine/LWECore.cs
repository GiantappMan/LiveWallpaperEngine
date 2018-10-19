//https://www.codeproject.com/Articles/856020/Draw-Behind-Desktop-Icons-in-Windows
using DZY.WinAPI;
using DZY.WinAPI.Desktop.API;
using System;

namespace LiveWallpaperEngine
{
    /// <summary>
    /// 动态壁纸核心逻辑，就这么两句。。没什么好藏在噎着的。代码我也是从互联网抄写的-。-
    /// </summary>
    public static class LWECore
    {
        #region fields
        static IntPtr _workerw;
        static bool _initlized;
        static IntPtr _targeHandler;
        static IntPtr _parentHandler;
        static IDesktopWallpaper _desktopWallpaperAPI = DesktopWallpaperFactory.Create();
        static bool _disableOSWallpaper;

        public static bool Shown { get; private set; }
        public static event EventHandler<IntPtr> ShownEvent;
        public static event EventHandler<IntPtr> UnshownEvent;

        #endregion

        public static void RestoreParent()
        {
            if (!Shown)
                return;

            if (!_initlized)
                Initlize();

            User32Wrapper.SetParent(_targeHandler, _parentHandler);
            if (_disableOSWallpaper)
                _desktopWallpaperAPI.Enable(true);
            Shown = false;
            UnshownEvent?.Invoke(null, _targeHandler);
        }

        public static bool SendToBackground(IntPtr handler, bool disableOSWallpaper = true)
        {
            if (handler == IntPtr.Zero || Shown)
                return false;

            Shown = true;
            _disableOSWallpaper = disableOSWallpaper;
            _targeHandler = handler;

            if (!_initlized)
            {
                bool isOk = Initlize();
                if (!isOk)
                    return false;
            }
            _parentHandler = User32Wrapper.GetParent(_targeHandler);
            if (_parentHandler == IntPtr.Zero)
                _parentHandler = User32Wrapper.GetAncestor(_targeHandler, GetAncestorFlags.GetParent);
            User32Wrapper.SetParent(_targeHandler, _workerw);
            if (_disableOSWallpaper)
                _desktopWallpaperAPI.Enable(false);
            ShownEvent?.Invoke(null, _targeHandler);
            return true;
        }

        private static bool Initlize()
        {
            IntPtr progman = User32Wrapper.FindWindow("Progman", null);
            IntPtr result = IntPtr.Zero;
            User32Wrapper.SendMessageTimeout(progman,
                                   0x052C,
                                   new IntPtr(0),
                                   IntPtr.Zero,
                                   SendMessageTimeoutFlags.SMTO_NORMAL,
                                   1000,
                                   out result);
            _workerw = IntPtr.Zero;
            var result1 = User32Wrapper.EnumWindows(new EnumWindowsProc((tophandle, topparamhandle) =>
            {
                IntPtr p = User32Wrapper.FindWindowEx(tophandle,
                                            IntPtr.Zero,
                                            "SHELLDLL_DefView",
                                            IntPtr.Zero);

                if (p != IntPtr.Zero)
                {
                    _workerw = User32Wrapper.FindWindowEx(IntPtr.Zero,
                                             tophandle,
                                             "WorkerW",
                                             IntPtr.Zero);
                }

                return true;
            }), IntPtr.Zero);
            _initlized = result1;
            return result1;
        }
    }
}
