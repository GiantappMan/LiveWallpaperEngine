using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using LiveWallpaperEngine.Common;

namespace LiveWallpaperEngineRender
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
        //public static void UIBeginInvoke(Action a)
        //{
        //    if (Application.OpenForms.Count == 0)
        //        return;

        //    var mainForm = Application.OpenForms[0];

        //    if (mainForm.InvokeRequired)
        //        mainForm.BeginInvoke(a);
        //    else
        //        a();
        //}
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
                Refresh();
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

        internal async void ShowWallpaper(Control control)
        {
            //WallpaperHelper.GetInstance(_screenIndex).RestoreParent();
            Load += RenderHost_Load;
            Controls.Clear();
            control.Dock = DockStyle.Fill;
            Controls.Add(control);
            Show();
            //Invalidate();

            await Task.Delay(200);
            Opacity = 1;
            Refresh();
            WallpaperHelper.GetInstance(_screenIndex).SendToBackground(Handle);
        }

        private void RenderHost_Load(object sender, EventArgs e)
        {
            Load -= RenderHost_Load;
            //WallpaperHelper.GetInstance(_screenIndex).SendToBackground(Handle);
            //WallpaperHelper.GetInstance(_screenIndex).SendToBackground(Handle);
        }


        //protected override void OnLoad(EventArgs e)
        //{
        //    base.OnLoad(e);
        //    WallpaperHelper.GetInstance(_screenIndex).SendToBackground(Handle);
        //}
    }
}
