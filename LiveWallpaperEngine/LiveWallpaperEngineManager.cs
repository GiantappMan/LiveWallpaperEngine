using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Threading;

namespace LiveWallpaperEngine
{
    public class LiveWallpaperEngineCoreEx : LiveWallpaperEngineCore
    {
        internal LiveWallpaperEngineCoreEx(Screen screen) : base(screen) { }
        public IRender Render { get; set; }
    }

    /// <summary>
    /// 管理多屏壁纸
    /// </summary>
    public class LiveWallpaperEngineManager
    {
        public static List<Screen> AllScreens { get; private set; }
        public static Dispatcher UIDispatcher { get; set; }

        static List<LiveWallpaperEngineCoreEx> cores = new List<LiveWallpaperEngineCoreEx>();
        static LiveWallpaperEngineManager()
        {
            ExplorerMonitor.ExpolrerCreated += _explorerMonitor_ExpolrerCreated;
            ExplorerMonitor.Start();

            SetupCores();
        }

        private static void _explorerMonitor_ExpolrerCreated(object sender, EventArgs e)
        {//explorer 重启
            cores.ForEach(m =>
            {
                m?.ReInit();
                if (m.Render != null)
                {
                    IntPtr handle = IntPtr.Zero; ;
                    handle = m.Render.RestartRender();
                    m.SendToBackground(handle);
                    m.Render.Play(m.Render.CurrentPath);
                }
            });
        }

        public static void SetupCores()
        {
            AllScreens = new List<Screen>(Screen.AllScreens);
            AllScreens.ForEach(item =>
             {
                 cores.Add(new LiveWallpaperEngineCoreEx(item));
             });
        }

        public static bool Show(IRender render, Screen screen)
        {
            var handle = render.ShowRender();

            var core = GetCore(screen);
            if (core == null)
                return false;

            core.Render = render;
            bool ok = core.SendToBackground(handle);
            return ok;
        }

        public static LiveWallpaperEngineCoreEx GetCore(Screen screen)
        {
            var result = cores.FirstOrDefault(m => m.DisplayScreen == screen);
            return result;
        }
    }
}
