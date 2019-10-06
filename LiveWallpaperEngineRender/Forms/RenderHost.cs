using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using LiveWallpaperEngine.Common;
using LiveWallpaperEngine.Common.Renders;

namespace LiveWallpaperEngineRender
{
    public partial class RenderHost : Form
    {
        static Dictionary<int, RenderHost> _hosts = new Dictionary<int, RenderHost>();
        int _screenIndex;
        IRender _currentRender;

        public RenderHost(int screenIndex)
        {
            _screenIndex = screenIndex;
            //UI
            BackColor = Color.Magenta;
            TransparencyKey = Color.Magenta;
            ShowInTaskbar = false;
            InitializeComponent();
            FormBorderStyle = FormBorderStyle.None;
            Opacity = 0;

            InitializeComponent();
            _hosts[screenIndex] = this;
        }

        public static void UIInvoke(Action a)
        {
            if (Application.OpenForms.Count == 0)
                return;

            var mainForm = Application.OpenForms[0];

            if (mainForm.InvokeRequired)
                mainForm.Invoke(a);
            else
                a();
        }

        internal void RemoveWallpaper(Control control)
        {
            UIInvoke(() =>
            {
                if (Controls.Contains(control))
                    Controls.Remove(control);
                Invalidate();
            });
        }

        public static RenderHost GetHost(int screenIndex = 0)
        {
            if (!_hosts.ContainsKey(screenIndex))
            {
                RenderHost result = null;
                UIInvoke(() =>
                {
                    result = new RenderHost(screenIndex);
                });
                return result;
            }

            return _hosts[screenIndex];
        }

        internal void ShowWallpaper(Control control, IRender render)
        {
            //关闭上一个壁纸
            _currentRender?.CloseWallpaper(_screenIndex);
            _currentRender = render;
            UIInvoke(() =>
            {
                Controls.Clear();
                control.Dock = DockStyle.Fill;
                Controls.Add(control);
                Opacity = 1;
                Show();
                WallpaperHelper.GetInstance(_screenIndex).SendToBackground(Handle);
                Invalidate();
            });
        }
    }
}
