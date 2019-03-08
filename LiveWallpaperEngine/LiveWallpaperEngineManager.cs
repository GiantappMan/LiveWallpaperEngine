using System;
using System.Collections.Generic;
using System.Text;

namespace LiveWallpaperEngine
{
    public class LiveWallpaperEngineManager
    {
        public static bool Show(IRender render)
        {
            var handle = render.ShowRender();
            bool ok = LiveWallpaperEngineCore.SendToBackground(handle, 0);
            return ok;
            //LiveWallpaperEngineCore.
        }
    }
}
