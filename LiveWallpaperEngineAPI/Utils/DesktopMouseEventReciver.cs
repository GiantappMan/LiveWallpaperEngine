// 参考ADD-SP的PR https://github.com/giant-app/LiveWallpaperEngine/pull/13 ，修改为c#版本
// 感谢https://github.com/ADD-SP 的提交
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using DZY.WinAPI;
using EventHook;

namespace Giantapp.LiveWallpaper.Engine.Utils
{
    /// <summary>
    /// 监听桌面鼠标消息
    /// </summary>
    public static class DesktopMouseEventReciver
    {
        private static EventHookFactory eventHookFactory = new EventHookFactory();
        private static MouseWatcher mouseWatcher;
        static DesktopMouseEventReciver()
        {
        }
        static bool started = false;

        public static List<IntPtr> HTargetWindows { get; internal set; } = new List<IntPtr>();

        internal static void Stop()
        {
            mouseWatcher?.Stop();
            started = false;
        }

        internal static void Start()
        {
            if (started)
                return;

            mouseWatcher = eventHookFactory.GetMouseWatcher();
            mouseWatcher.Start();
            mouseWatcher.OnMouseInput += (s, e) =>
            {
                // 根据官网文档中定义，lParam低16位存储鼠标的x坐标，高16位存储y坐标
                int lParam = e.Point.y;
                lParam <<= 16;
                lParam |= e.Point.x;
                // 发送消息给目标窗口
                foreach (var window in HTargetWindows)
                    User32Wrapper.PostMessageW(window, (uint)e.Message, (IntPtr)0x0020, (IntPtr)lParam);

                Console.WriteLine("Mouse event {0} at point {1},{2}", e.Message.ToString(), e.Point.x, e.Point.y);
            };

            started = true;
        }
    }
}
