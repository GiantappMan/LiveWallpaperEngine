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
        static Dictionary<int, RenderHost> _hosts = new Dictionary<int, RenderHost>();
        int _screenIndex;

        public RenderHost(int screenIndex)
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
        public static void MainUIInvoke(Action a)
        {
            if (Application.OpenForms.Count == 0)
                return;

            var mainForm = Application.OpenForms[0];
            mainForm.InvokeIfRequired(a);            
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

        public static RenderHost GetHost(int screenIndex = 0)
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
