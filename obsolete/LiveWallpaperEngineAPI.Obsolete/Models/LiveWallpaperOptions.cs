using System.Collections.Generic;

namespace Giantapp.LiveWallpaper.Engine.Models
{
    public class LiveWallpaperOptions
    {
        public bool AutoRestartWhenExplorerCrash { get; set; }
        public List<ScreenOption> ScreenOptions { get; set; }
        /// <summary>
        /// -1 表示禁用
        /// </summary>
        public int AudioScreenIndex { get; set; }
        public bool AppMaximizedEffectAllScreen { get; set; }
    }
}
