using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace LiveWallpaperEngine
{
    /// <summary>
    /// 监控 explorer是否崩溃
    /// </summary>
    public static class ExplorerMonitor
    {
        private static System.Timers.Timer _timer;
        private static DateTime? _lastTriggerTime;

        public static Process ExploreProcess { get; private set; }

        public static event EventHandler ExpolrerCreated;

        static ExplorerMonitor()
        {
            ExploreProcess = GetExplorer();
        }

        public static void Start()
        {
            if (_timer == null)
            {
                _timer = new System.Timers.Timer(1000);
                _timer.Elapsed += _timer_Elapsed;
                _timer.Start();
            }
        }

        public static void Stop()
        {
            if (_timer != null)
            {
                _timer.Stop();
                _timer.Elapsed -= _timer_Elapsed;
            }
            _timer = null;
        }

        private static void _timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            _timer.Stop();

            //explorer 进程已死
            if (ExploreProcess == null || ExploreProcess.HasExited)
            {
                ExploreProcess = GetExplorer();
                _lastTriggerTime = DateTime.Now;
            }

            if (_lastTriggerTime != null)
            {
                var workw = LiveWallpaperEngineCore.GetWorkerW();
                if (workw != IntPtr.Zero)
                {
                    ExpolrerCreated?.Invoke(null, new EventArgs());
                    _lastTriggerTime = null;
                }
            }

            if (_timer == null)
                return;//disposed
            _timer.Start();
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
