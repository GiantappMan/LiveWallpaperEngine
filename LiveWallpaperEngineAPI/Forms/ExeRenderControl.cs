using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Giantapp.LiveWallpaper.Engine.Renders;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Windows.Media;
using Giantapp.LiveWallpaper.Engine.Common;
using DZY.WinAPI;

namespace Giantapp.LiveWallpaper.Engine.Forms
{
    //显示exe的容器
    public partial class ExeRenderControl : UserControl, IRenderControl
    {

        private int _currentPid;
        private IntPtr _currentTargetHandle;

        public ExeRenderControl()
        {
            InitializeComponent();
        }

        public void DisposeRender()
        {
        }

        public void InitRender()
        {
        }

        public void Pause()
        {
        }

        public void Resum()
        {
        }

        public void SetVolume(int volume)
        {
        }

        public void Stop()
        {
            try
            {
                var p = Process.GetProcessById(_currentPid);
                if (p == null)
                    return;


                if (_currentTargetHandle != IntPtr.Zero)
                {
                    DesktopMouseEventReciver.HTargetWindows.Remove(_currentTargetHandle);
                }

                p.Kill();
            }
            catch (Exception ex)
            {

            }
        }

        void IRenderControl.Load(string path)
        {
            IntPtr handle = IntPtr.Zero;
            RenderHost.MainUIInvoke(() =>
            {
                handle = Handle;
            });
            LoadApplication(path, handle);
        }

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);
        [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
        public static extern IntPtr SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int x, int Y, int cx, int cy, int wFlags);

        private void LoadApplication(string path, IntPtr containerHandle)
        {
            try
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                int timeout = 10 * 1000;     // Timeout value (10s) in case we want to cancel the task if it's taking too long.

                ProcessStartInfo info = new ProcessStartInfo(path);
                info.WindowStyle = ProcessWindowStyle.Maximized;
                info.CreateNoWindow = true;
                Process targetProcess = Process.Start(info);
                while (targetProcess.MainWindowHandle == IntPtr.Zero)
                {
                    System.Threading.Thread.Sleep(10);
                    int pid = targetProcess.Id;
                    targetProcess.Dispose();
                    //mainWindowHandle不会变，重新获取
                    targetProcess = Process.GetProcessById(pid);

                    if (sw.ElapsedMilliseconds > timeout)
                    {
                        sw.Stop();
                        return;
                    }
                }

                _currentTargetHandle = targetProcess.MainWindowHandle;
                _currentPid = targetProcess.Id;

                DesktopMouseEventReciver.HTargetWindows.Add(_currentTargetHandle);

                // 用当前窗口显示exe
                SetParent(_currentTargetHandle, containerHandle);
                WallpaperHelper.FullScreen(_currentTargetHandle, containerHandle);
            }
            catch (Exception ex)
            {
            }
        }
    }
}
