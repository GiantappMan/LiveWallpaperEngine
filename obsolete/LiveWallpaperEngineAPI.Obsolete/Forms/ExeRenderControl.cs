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
using DZY.WinAPI.Extension;

namespace Giantapp.LiveWallpaper.Engine.Forms
{
 
    //显示exe的容器
    public partial class ExeRenderControl : UserControl, IRenderControl
    {
        private int _currentPid = -1;
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
            var p = Process.GetProcessById(_currentPid);
            p.Suspend();
        }

        public void Resum()
        {
            var p = Process.GetProcessById(_currentPid);
            p.Resume();
        }

        public void SetVolume(int volume)
        {
        }

        public void Stop()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("stop exe 0");
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
            finally
            {
                System.Diagnostics.Debug.WriteLine("stop exe 1");
                _currentPid = -1;
            }
        }

        void IRenderControl.Load(string path)
        {
            // 当前已有壁纸 过滤
            if (_currentPid >= 0)
                return;
            System.Diagnostics.Debug.WriteLine("load exe0");
            IntPtr handle = IntPtr.Zero;
            WallpaperManager.UIInvoke(() =>
            {
                handle = Handle;
            });
            LoadApplication(path, handle);
            System.Diagnostics.Debug.WriteLine("load exe1");
        }

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
                User32Wrapper.SetParent(_currentTargetHandle, containerHandle);
                WallpaperHelper.FullScreen(_currentTargetHandle, containerHandle);
            }
            catch (Exception ex)
            {
            }
        }
    }
}
