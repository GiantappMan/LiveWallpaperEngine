// 参考ADD-SP的PR https://github.com/giant-app/LiveWallpaperEngine/pull/13 ，修改为c#版本
// 感谢https://github.com/ADD-SP 的提交
using System;
using System.Collections;
using System.Collections.Generic;
using DZY.WinAPI;
using EventHook;

namespace Giantapp.LiveWallpaper.Engine.Utils
{
    /// <summary>
    /// 监听桌面鼠标消息
    /// </summary>
    public static class DesktopMouseEventReciver
    {
        private static List<IntPtr> _targetWindows = new List<IntPtr>();
        private static EventHookFactory eventHookFactory = new EventHookFactory();
        private static MouseWatcher mouseWatcher;
        private static bool started = false;
        private static DateTime _nextSendTime;

        //转发间隔，防止消阻塞
        public static int SendInterval { get; set; } = 0;

        public static void AddHandle(IntPtr handle)
        {
            var threadSafeList = ArrayList.Synchronized(_targetWindows);
            threadSafeList.Add(handle);
        }
        public static void RemoveHandle(IntPtr handle)
        {
            var threadSafeList = ArrayList.Synchronized(_targetWindows);
            threadSafeList.Remove(handle);
        }
        public static void Stop()
        {
            mouseWatcher?.Stop();
            started = false;
        }

        public static void Start()
        {
            if (started)
                return;

            mouseWatcher = eventHookFactory.GetMouseWatcher();
            mouseWatcher.Start();
            mouseWatcher.OnMouseInput += (s, e) =>
            {
                if (SendInterval > 0)
                {
                    if (DateTime.Now < _nextSendTime)
                        return;

                    _nextSendTime = DateTime.Now + TimeSpan.FromMilliseconds(SendInterval);
                }

                // 根据官网文档中定义，lParam低16位存储鼠标的x坐标，高16位存储y坐标
                int lParam = e.Point.y;
                lParam <<= 16;
                lParam |= e.Point.x;
                // 发送消息给目标窗口

                IntPtr wParam = (IntPtr)0x0020;
                //switch (e.Message)
                //{
                //    case EventHook.Hooks.MouseMessages.WM_LBUTTONDOWN:
                //    case EventHook.Hooks.MouseMessages.WM_LBUTTONUP:
                //        wParam = (IntPtr)0x0001;
                //        break;
                //    case EventHook.Hooks.MouseMessages.WM_MOUSEMOVE:
                //        wParam = (IntPtr)0x0020;
                //        break;
                //    case EventHook.Hooks.MouseMessages.WM_RBUTTONDOWN:
                //        wParam = (IntPtr)0x0002;
                //        break;
                //}

                //if (wParam == IntPtr.Zero)
                //    return;

                //todo cef收到鼠标事件后会自动active，看有办法规避不
                foreach (IntPtr window in _targetWindows)
                    User32Wrapper.PostMessageW(window, (uint)e.Message, wParam, (IntPtr)lParam);

                System.Diagnostics.Debug.WriteLine("Mouse event {0} at point {1},{2}", e.Message.ToString(), e.Point.x, e.Point.y);
            };

            started = true;
        }
    }
}
