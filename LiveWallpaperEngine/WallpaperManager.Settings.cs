﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveWallpaperEngine
{
    public enum ActionWhenMaximized
    {
        Play,
        Pause,
        Stop
    }
    public struct ScreenSetting
    {
        public int ScreenIndex { get; set; }
        /// <summary>
        /// 当前窗口其他程序全屏时
        /// </summary>
        public ActionWhenMaximized WhenCurrentScreenMaximized { get; set; }
    }

    public struct Settings
    {
        /// <summary>
        /// 播放音频的显示器
        /// </summary>
        public static int AudioScreenIndex { get; set; }
        /// <summary>
        /// 每个屏幕的单独设置
        /// </summary>
        public static ScreenSetting[] ScreenSettings { get; set; }
    }
}