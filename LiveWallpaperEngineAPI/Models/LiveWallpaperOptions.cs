using System.Collections.Generic;

namespace LiveWallpaperEngineAPI.Models
{
    public class LiveWallpaperOptions
    {
        public bool AutoRestartWhenExplorerCrash { get; set; }
        public List<ScreenOption> ScreenOptions { get; set; }
        public uint AudioScreenIndex { get; set; }
        public bool AppMaximizedEffectAllScreen { get; set; }
    }
}
