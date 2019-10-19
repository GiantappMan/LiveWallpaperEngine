using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using DZY.Util.Winform.Extensions;
using LiveWallpaperEngineAPI.Common;

namespace LiveWallpaperEngineAPI
{
    public partial class RenderHost : Form
    {
        static Dictionary<uint, RenderHost> _hosts = new Dictionary<uint, RenderHost>();
        uint _screenIndex;

        public RenderHost(uint screenIndex)
        {
            InitializeComponent();
            Text = "RenderHost" + screenIndex;
            _screenIndex = screenIndex;
            //UI
            BackColor = Color.Magenta;
            TransparencyKey = Color.Magenta;
            ShowInTaskbar = false;
            FormBorderStyle = FormBorderStyle.None;
            Opacity = 0;

            _hosts[screenIndex] = this;
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
        }

        public static void WinformInvoke(Action a)
        {
            if (Application.OpenForms.Count == 0)
                return;

            var mainForm = Application.OpenForms[0];
            mainForm.InvokeIfRequired(a);
        }

        public static void WPFInvoke(Action a)
        {
            var mainWindow = System.Windows.Application.Current.MainWindow;
            if (mainWindow == null)
                return;

            if (mainWindow.Dispatcher.CheckAccess()) // CheckAccess returns true if you're on the dispatcher thread
                mainWindow.Dispatcher.Invoke(a);
            else
                a();
        }

        public static void MainUIInvoke(Action a)
        {
            if (Application.OpenForms.Count == 0)
                WPFInvoke(a);
            else
                WinformInvoke(a);
        }

        internal void RemoveWallpaper(Control control)
        {
            MainUIInvoke(() =>
            {
                if (Controls.Contains(control))
                    Controls.Remove(control);
                Refresh();
            });
        }

        public static RenderHost GetHost(uint screenIndex = 0)
        {
            if (!_hosts.ContainsKey(screenIndex))
            {
                MainUIInvoke(() =>
                {
                    var host = _hosts[screenIndex] = new RenderHost(screenIndex);
                    host.Show();
                });
            }

            return _hosts[screenIndex];
        }

        internal void ShowWallpaper(Control control)
        {
            IntPtr windowHandle = IntPtr.Zero;
            this.InvokeIfRequired(() =>
            {
                Controls.Clear();
                control.Dock = DockStyle.Fill;
                Controls.Add(control);
                Opacity = 1;
                Refresh();
                windowHandle = Handle;
            });
            WallpaperHelper.GetInstance(_screenIndex).SendToBackground(windowHandle);
        }
    }
}
