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
        private string _lastUrl = null;

        public WebControl()
        {
            InitializeComponent();
            _browser = new ChromiumWebBrowser(address: null)
            {
                Dock = DockStyle.Fill,
            };
            _browser.IsBrowserInitializedChanged += Browser_IsBrowserInitializedChanged;
            Controls.Add(_browser);
        }
   
        private void Browser_IsBrowserInitializedChanged(object sender, EventArgs e)
        {
            _browser.IsBrowserInitializedChanged -= Browser_IsBrowserInitializedChanged;
            if (_lastUrl != null)
                Invoke(new Action(() =>
                {
                    _browser.Load(_lastUrl);
                }));
        }

        public void InitRender()
        {
        }

        void IRenderControl.Load(string url)
        {
            if (!_browser.IsBrowserInitialized)
            {
                _lastUrl = url;
                return;
            }
            Invoke(new Action(() =>
            {
                _browser.Load(url);
            }));
        }

        public void Stop()
        {
            _browser?.LoadHtml("");
        }

        public void Pause()
        {
            throw new NotImplementedException();
        }

        public void Resum()
        {
            throw new NotImplementedException();
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
