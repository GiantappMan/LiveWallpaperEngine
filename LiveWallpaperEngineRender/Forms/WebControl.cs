using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using CefSharp.WinForms;
using CefSharp;
using System.IO;
using LiveWallpaperEngine.Common.Renders;

namespace LiveWallpaperEngineRender.Forms
{
    public partial class WebControl : UserControl, IRenderControl
    {
        static WebControl()
        {
            CefSharpSettings.SubprocessExitIfParentProcessClosed = true;
            Cef.EnableHighDPISupport();
            var settings = new CefSettings()
            {
                CachePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "CefSharp\\Cache"),
                WindowlessRenderingEnabled = true
            };
            settings.CefCommandLineArgs.Add("enable-media-stream", "1");
            Cef.Initialize(settings, performDependencyCheck: true, browserProcessHandler: null);
        }

        private ChromiumWebBrowser _browser;

        public WebControl()
        {
            InitializeComponent();
            //UI
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            SetStyle(ControlStyles.Opaque, true);
            BackColor = Color.Transparent;
        }

        public void InitRender()
        {
        }

        void IRenderControl.Load(string url)
        {
            if (_browser == null || !_browser.IsBrowserInitialized)
            {
                _browser = new ChromiumWebBrowser(address: url)
                {
                    Dock = DockStyle.Fill,
                };

                BeginInvoke(new Action(() =>
                {
                    Controls.Add(_browser);
                    Refresh();
                }));
            }
            else
                BeginInvoke(new Action(() =>
                {
                    _browser.Load(url);
                }));
        }

        public void Stop()
        {
            if (IsDisposed)
                return;
            BeginInvoke(new Action(() =>
            {
                _browser?.Dispose();
                _browser = null;
            }));
        }

        public void Pause()
        {
            //throw new NotImplementedException();
        }

        public void Resum()
        {
            //throw new NotImplementedException();
        }

        public void DisposeRender()
        {
            //_browser.Dispose();
            _browser = null;
        }

        public void SetVolume(int volume)
        {
            throw new NotImplementedException();
        }
    }
}
