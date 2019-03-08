//原理参考
//https://www.codeproject.com/Articles/856020/Draw-Behind-Desktop-Icons-in-Windows 
//https://github.com/Francesco149/weebp/blob/master/src/weebp.c 
using DZY.WinAPI;
using DZY.WinAPI.Desktop.API;
using System;
using System.Diagnostics;
using System.Timers;

namespace LiveWallpaperEngine
{
    /// <summary>
    /// 动态壁纸实现原理
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

        Process _exploreProcess;
        Timer _timer;

        //public properties
        public bool Shown { get; private set; }
        public object Screen { get; private set; }
        //event
        public event EventHandler TimerElapsed;
        public event EventHandler NeedReapply;

        #endregion

        #region construct

        public LWECore()
        {
            _timer = new Timer(1000);
            _timer.Elapsed += _timer_Elapsed;
            _timer.Start();

            InnerInit();
        }

        private void InnerInit()
        {
            _exploreProcess = GetExplorer();

            var _desktopWallpaperAPI = GetDesktopWallpaperAPI();
            _desktopWallpaperAPI?.GetSlideshowOptions(out DesktopSlideshowOptions temp, out _slideshowTick);
            _desktopWallpaperAPI?.SetSlideshowOptions(DesktopSlideshowOptions.DSO_SHUFFLEIMAGES, 1000 * 60 * 60 * 24);
        }

        private void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            _timer.Stop();

            //explorer 进程已死
            if (_exploreProcess == null || _exploreProcess.HasExited)
            {
                _workerw = IntPtr.Zero;
                _exploreProcess = GetExplorer();
            }

            //重新应用壁纸
            if (Shown && _exploreProcess != null && _workerw == IntPtr.Zero)
            {
                InnerInit();
                _workerw = GetWorkerW();
                Shown = false;
                NeedReapply?.Invoke(this, new EventArgs());
            }

            TimerElapsed?.Invoke(this, new EventArgs());

            if (_timer == null)
                return;//disposed
            _timer.Start();
        }

        public void Dispose()
        {
            _timer.Elapsed -= _timer_Elapsed;
            _timer.Stop();
            _timer = null;
            _desktopWallpaperAPI?.SetSlideshowOptions(DesktopSlideshowOptions.DSO_SHUFFLEIMAGES, _slideshowTick);
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

            RefreshWallpaper(null);
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

            _desktopWallpaperAPI = RefreshWallpaper(_desktopWallpaperAPI);

            if (_workerw == IntPtr.Zero)
                _workerw = GetWorkerW();

            User32Wrapper.SetParent(_targeHandler, _parentHandler);

            if (_originalRect != null)
                User32Wrapper.SetWindowPos(_targeHandler, _originalRect.Value);
            Shown = false;
        }

        public bool SendToBackground(IntPtr handler, int displayIndex = 0)
        {
            if (handler == IntPtr.Zero || Shown)
                return false;

            var ok = User32Wrapper.GetWindowRect(handler, out RECT react);
            if (ok)
                _originalRect = react;

            Shown = true;
            _targeHandler = handler;

            if (_workerw == IntPtr.Zero)
            {
                _workerw = GetWorkerW();
                if (_workerw == IntPtr.Zero)
                    return false;
            }

            _parentHandler = User32Wrapper.GetParent(_targeHandler);
            if (_parentHandler == IntPtr.Zero)
                _parentHandler = User32Wrapper.GetAncestor(_targeHandler, GetAncestorFlags.GetParent);

            User32Wrapper.SetParent(_targeHandler, _workerw);

            FullScreen(_targeHandler, displayIndex);

            return true;
        }

        public static Process GetExplorer()
        {
            var explorers = Process.GetProcessesByName("explorer");
            if (explorers.Length == 0)
            {
                //还是不自动启动了，有点像流氓行为
                //string explorer = string.Format("{0}\\{1}", Environment.GetEnvironmentVariable("WINDIR"), "explorer.exe");
                //Process process = new Process();
                //process.StartInfo.FileName = explorer;
                //process.StartInfo.UseShellExecute = true;
                //process.Start();
                //return process;
                return null;
            }

            return explorers[0];
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

        //刷新壁纸
        private static IDesktopWallpaper RefreshWallpaper(IDesktopWallpaper desktopWallpaperAPI)
        {
            var explorer = GetExplorer();
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
