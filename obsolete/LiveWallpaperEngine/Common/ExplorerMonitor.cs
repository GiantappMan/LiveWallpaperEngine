using System;
using System.Diagnostics;

namespace LiveWallpaperEngine.Common
{
    /// <summary>
    /// 监控 explorer是否崩溃
    /// </summary>
    public static class ExplorerMonitor
    {
        private static DateTime? _lastTriggerTime;

        public static Process ExploreProcess { get; private set; }

        public static event EventHandler ExpolrerCreated;

        static ExplorerMonitor()
        {
            ExploreProcess = GetExplorer();
        }

        public static void Check()
        {
            //explorer 进程已死
            if (ExploreProcess == null || ExploreProcess.HasExited)
            {
                ExploreProcess = GetExplorer();
                _lastTriggerTime = DateTime.Now;
            }

            if (_lastTriggerTime != null)
            {
                var workw = WallpaperHelper.GetWorkerW();
                if (workw != IntPtr.Zero)
                {
                    ExpolrerCreated?.Invoke(null, new EventArgs());
                    _lastTriggerTime = null;
                }
            }
        }

        private static Process GetExplorer()
        {
            var explorers = Process.GetProcessesByName("explorer");
            if (explorers.Length == 0)
            {
                return null;
            }

            return explorers[0];
        }
    }
}
