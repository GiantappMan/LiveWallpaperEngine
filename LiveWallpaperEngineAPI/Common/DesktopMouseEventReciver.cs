// 参考ADD-SP的PR https://github.com/giant-app/LiveWallpaperEngine/pull/13 ，修改为c#版本
// 感谢https://github.com/ADD-SP 的提交
using DZY.WinAPI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace LiveWallpaperEngineAPI.Common
{
    /// <summary>
    /// 监听桌面鼠标消息
    /// </summary>
    public static class DesktopMouseEventReciver
    {
        const int WH_MOUSE = 7;
        public static bool InstallHook()
        {
            var desktop = GetDesktop();
            if (desktop == IntPtr.Zero)
                return false;

            var dwThreadId = User32Wrapper.GetWindowThreadProcessId(desktop, out int _);

            IntPtr dllIns = Kernel32Wrapper.GetModuleHandle("user32");
            var hhookR = User32Wrapper.SetWindowsHookEx(WH_MOUSE, MouseProc, dllIns, 0);
            return hhookR > 0;
        }

        private static int MouseProc(int code, int wParam, IntPtr lParam)
        {
            return User32Wrapper.CallNextHookEx(0, code, wParam, lParam);
        }

        public static IntPtr GetDesktop()
        {
            var hWndProgMan = User32Wrapper.FindWindow("ProgMan", null);
            if (hWndProgMan != IntPtr.Zero)
            {
                var hWndShellDefView = User32Wrapper.FindWindowEx(hWndProgMan, IntPtr.Zero, "SHELLDLL_DefView", IntPtr.Zero);
                if (hWndShellDefView != IntPtr.Zero)
                {
                    var hWndDesktop = User32Wrapper.FindWindowEx(hWndShellDefView, IntPtr.Zero, "SysListView32", IntPtr.Zero);
                    return hWndDesktop;
                }
            }

            return IntPtr.Zero;
        }
    }
}
