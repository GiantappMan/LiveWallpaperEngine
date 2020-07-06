using System;
using System.Collections.Generic;
using System.Text;

namespace Giantapp.LiveWallpaper.Engine
{
    public class ScreenOption
    {
        public enum ActionWhenMaximized
        {
            Pause,
            Stop,
            Play
        }
        /// <summary>
        /// 最大化时壁纸是否暂停，停止等
        /// </summary>
        public ActionWhenMaximized WhenAppMaximized { get; set; }
        /// <summary>
        /// 所影响的屏幕
        /// </summary>
        public string Screen { get; set; }
    }
    /// <summary>
    /// api 提供的选项
    /// </summary>
    public class LiveWallpaperOptions
    {   
        /// <summary>
        /// explorer挂了是否重启
        /// </summary>
        public bool AutoRestartWhenExplorerCrash { get; set; }
        /// <summary>
        /// 屏幕参数
        /// </summary>
        public List<ScreenOption> ScreenOptions { get; set; }
        /// <summary>
        /// 壁纸音源来源哪块屏幕， -1 表示禁用
        /// </summary>
        public string AudioScreen { get; set; }
        /// <summary>
        /// 屏幕最大化是否影响所有屏幕
        /// </summary>
        public bool AppMaximizedEffectAllScreen { get; set; }
    }
}
