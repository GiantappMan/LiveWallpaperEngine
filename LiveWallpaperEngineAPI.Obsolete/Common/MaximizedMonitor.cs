using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;

namespace Giantapp.LiveWallpaper.Engine.Common
{
    public class AppMaximizedEvent : EventArgs
    {
        public List<Screen> MaximizedScreens { get; set; }
    }

    public static class MaximizedMonitor
    {
        static Process _cp;
        static List<Screen> maximizedScreens = new List<Screen>();

        public static event EventHandler<AppMaximizedEvent> AppMaximized;

        public static void Check()
        {
            if (_cp == null)
                _cp = Process.GetCurrentProcess();

            new DZY.WinAPI.Helpers.OtherProgramChecker(_cp.Id).CheckMaximized(out List<Screen> fullscreenWindow);
            if (maximizedScreens.Count == fullscreenWindow.Count)
                return;

            maximizedScreens = fullscreenWindow;

            AppMaximized?.Invoke(null, new AppMaximizedEvent()
            {
                MaximizedScreens = maximizedScreens
            });
        }
    }
}
