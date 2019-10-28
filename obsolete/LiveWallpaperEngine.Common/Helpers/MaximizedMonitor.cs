using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;

namespace LiveWallpaperEngine.Common
{
    public class AppMaximizedEvent : EventArgs
    {
        public Screen MaximizedScreen { get; set; }

        public bool Maximized { get; set; }
    }

    public static class MaximizedMonitor
    {
        static Process _cp;
        static bool _maximized;

        public static event EventHandler<AppMaximizedEvent> AppMaximized;

        public static void Check()
        {
            if (_cp == null)
                _cp = Process.GetCurrentProcess();

            bool isMaximized = new DZY.WinAPI.Helpers.OtherProgramChecker(_cp.Id).CheckMaximized(out IntPtr fullscreenWindow);
            if (_maximized == isMaximized)
                return;

            _maximized = isMaximized;

            Screen maximizedScreen = Screen.FromHandle(fullscreenWindow);
            AppMaximized?.Invoke(null, new AppMaximizedEvent()
            {
                Maximized = _maximized,
                MaximizedScreen = maximizedScreen
            });
        }
    }
}
