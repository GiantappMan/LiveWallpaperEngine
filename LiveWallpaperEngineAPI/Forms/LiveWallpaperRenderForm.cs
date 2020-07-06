using DZY.WinAPI;
using Giantapp.LiveWallpaper.Engine.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Giantapp.LiveWallpaper.Engine.Forms
{
    /// <summary>
    /// 显示壁纸根窗体
    /// </summary>
    public partial class LiveWallpaperRenderForm : Form
    {
        static Dictionary<string, LiveWallpaperRenderForm> _hosts = new Dictionary<string, LiveWallpaperRenderForm>();
        string _screenName;
        IntPtr _currentWallpaperHandle;

        public LiveWallpaperRenderForm(string screenName)
        {
            InitializeComponent();
            Text = "RenderHost" + screenName;
            _screenName = screenName;
            //UI
            BackColor = Color.Magenta;
            TransparencyKey = Color.Magenta;
            ShowInTaskbar = false;
            FormBorderStyle = FormBorderStyle.None;
            Opacity = 0;
            _hosts[screenName] = this;
        }

        internal void ShowWallpaper(IntPtr wallpaperHandle)
        {
            IntPtr windowHandle = IntPtr.Zero;
            WallpaperManager.UIInvoke(() =>
            {
                try
                {
                    Controls.Clear();
                    Opacity = 1;
                    windowHandle = Handle;
                }
                catch (Exception ex)
                {

                }
            });
            WallpaperHelper.GetInstance(_screenName).SendToBackground(windowHandle);
            User32Wrapper.SetParent(wallpaperHandle, windowHandle);
        }

        public static LiveWallpaperRenderForm GetHost(string screen, bool autoCreate = true)
        {
            if (!_hosts.ContainsKey(screen))
            {
                if (autoCreate)
                    WallpaperManager.UIInvoke(() =>
                    {
                        var host = _hosts[screen] = new LiveWallpaperRenderForm(screen);
                        host.Show();
                    });
            }

            if (_hosts.ContainsKey(screen))
                return _hosts[screen];
            else
                return null;
        }

        #region private

        #endregion
    }
}
