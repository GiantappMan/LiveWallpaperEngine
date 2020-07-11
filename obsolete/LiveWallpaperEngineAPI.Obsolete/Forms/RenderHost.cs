using DZY.Util.Winform.Extensions;
using Giantapp.LiveWallpaper.Engine.Common;
using Giantapp.LiveWallpaper.Engine.Forms;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Giantapp.LiveWallpaper.Engine
{
    /// <summary>
    /// 显示壁纸根窗体
    /// </summary>
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

        internal void RemoveWallpaper(Control control)
        {
            WallpaperManager.UIInvoke(() =>
            {
                if (Controls.Contains(control))
                    Controls.Remove(control);

                if (control is IRenderControl r)
                    r.DisposeRender();
                control.Dispose();

                Refresh();
            });
        }

        internal void ShowWallpaper(Control renderControl)
        {
            IntPtr windowHandle = IntPtr.Zero;
            WallpaperManager.UIInvoke(() =>
            {
                try
                {
                    Controls.Clear();
                    renderControl.Dock = DockStyle.Fill;
                    Controls.Add(renderControl);
                    Opacity = 1;
                    Refresh();
                    windowHandle = Handle;
                }
                catch (Exception ex)
                {

                }
            });
            WallpaperHelper.GetInstance(_screenIndex).SendToBackground(windowHandle);
        }

        public static RenderHost GetHost(uint screenIndex = 0, bool autoCreate = true)
        {
            if (!_hosts.ContainsKey(screenIndex))
            {
                if (autoCreate)
                    WallpaperManager.UIInvoke(() =>
                    {
                        var host = _hosts[screenIndex] = new RenderHost(screenIndex);
                        host.Show();
                    });
            }

            if (_hosts.ContainsKey(screenIndex))
                return _hosts[screenIndex];
            else
                return null;
        }

        #region private

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
        }

        #endregion
    }
}
