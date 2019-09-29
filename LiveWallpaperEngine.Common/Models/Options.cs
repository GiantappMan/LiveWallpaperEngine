using System.Collections.Generic;

namespace LiveWallpaperEngine.Common.Models
{
    public enum ActionWhenMaximized
    {
        Play,
        Pause,
        Stop
    }
    public struct ScreenOption
    {
        public int ScreenIndex { get; set; }
        /// <summary>
        /// 当前窗口其他程序全屏时
        /// </summary>
        public ActionWhenMaximized? WhenCurrentScreenMaximized { get; set; }
    }

    public struct LiveWallpaperOptions
    {
        /// <summary>
        /// 播放音频的显示器
        /// </summary>
        public int? AudioScreenIndex { get; set; }
        /// <summary>
        /// 每个屏幕的单独设置
        /// </summary>
        public List<ScreenOption> ScreenOptions { get; set; }
        /// <summary>
        /// 当explor崩溃后自动重启
        /// </summary>
        public bool? AutoRestartWhenExplorerCrash { get; set; }
    }
}
