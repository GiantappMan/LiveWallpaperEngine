using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Threading;
using DZY.WinAPI;

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

        #region public methods

        public static bool Show(IRender render, Screen screen)
        {
            var handle = render.ShowRender();

            var core = GetCore(screen);
            if (core == null)
                return false;

            Close(core.Render);//清理旧Render

            render.SetCore(core);
            core.Render = render;

            //处理alt+tab可以看见本程序
            //https://stackoverflow.com/questions/357076/best-way-to-hide-a-window-from-the-alt-tab-program-switcher
            int exStyle = User32Wrapper.GetWindowLong(handle, WindowLongFlags.GWL_EXSTYLE);
            exStyle |= (int)WindowStyles.WS_EX_TOOLWINDOW;
            User32Wrapper.SetWindowLong(handle, WindowLongFlags.GWL_EXSTYLE, exStyle);

            bool ok = core.SendToBackground(handle);
            return ok;
        }

        public static void Close(IRender render)
        {
            render?.CloseRender();
            render?.SetCore(null);
            var core = GetCore(render);
            if (core == null)
                return;

            core.Render = null;
            core.Close();
        }

        public static LiveWallpaperEngineCoreEx GetCore(Screen screen)
        {
            var result = cores.FirstOrDefault(m => m.DisplayScreen == screen);
            return result;
        }

        #endregion


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
                    if (m.Render is IVideoRender)
                    {
                        var tmp = m.Render as IVideoRender;
                        tmp.Play(tmp.CurrentPath);
                    }
                }
            });
        }

        private static void SetupCores()
        {
            User32WrapperEx.SetThreadAwarenessContext(DPI_AWARENESS_CONTEXT.DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE);
            AllScreens = new List<Screen>(Screen.AllScreens);
            AllScreens.ForEach(item =>
             {
                 cores.Add(new LiveWallpaperEngineCoreEx(item));
             });
        }

        private static LiveWallpaperEngineCoreEx GetCore(IRender render)
        {
            var result = cores.FirstOrDefault(m => m.Render == render);
            return result;
        }

    }
}
