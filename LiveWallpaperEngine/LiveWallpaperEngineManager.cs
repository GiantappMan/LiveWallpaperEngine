using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Windows.Forms;

namespace LiveWallpaperEngine
{
    public class LiveWallpaperEngineManager
    {
        public static List<Screen> AllScreens { get; private set; }

        static List<LiveWallpaperEngineCore> cores = new List<LiveWallpaperEngineCore>();
        static LiveWallpaperEngineManager()
        {
            SetupCores();
        }

        public static void SetupCores()
        {
            AllScreens = new List<Screen>(Screen.AllScreens);
            AllScreens.ForEach(item =>
             {
                 cores.Add(new LiveWallpaperEngineCore(item));
             });
        }

        public static bool Show(IRender render, Screen screen)
        {
            var handle = render.ShowRender();

            var core = GetCore(screen);
            if (core == null)
                return false;

            bool ok = core.SendToBackground(handle);
            return ok;
        }

        public static LiveWallpaperEngineCore GetCore(Screen screen)
        {
            var result = cores.FirstOrDefault(m => m.DisplayScreen == screen);
            return result;
        }
    }
}
