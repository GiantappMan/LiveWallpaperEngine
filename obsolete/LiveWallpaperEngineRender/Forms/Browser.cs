//using Microsoft.Toolkit.Forms.UI.Controls;
using CefSharp;
using CefSharp.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace LiveWallpaperEngineRender
{
    public partial class Browser : Form
    {

        static Browser()
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

        private ChromiumWebBrowser browser;
        private string _lastUrl = null;
        public Browser()
        {
            //UI
            BackColor = Color.Magenta;
            TransparencyKey = Color.Magenta;
            ShowInTaskbar = false;
            FormBorderStyle = FormBorderStyle.None;

            InitializeComponent();
            browser = new ChromiumWebBrowser(address: @"F:/work/gitee/LiveWallpaperEngine/LiveWallpaperEngineRender/defaultHtml/index.html
")
            {
                Dock = DockStyle.Fill,
            };
            browser.IsBrowserInitializedChanged += Browser_IsBrowserInitializedChanged;

            Controls.Add(browser);
            WindowState = FormWindowState.Maximized;
        }

        private void Browser_IsBrowserInitializedChanged(object sender, EventArgs e)
        {
            browser.IsBrowserInitializedChanged -= Browser_IsBrowserInitializedChanged;
            if (_lastUrl != null)
                Invoke(new Action(() =>
                {
                    browser.Load(_lastUrl);
                }));
        }

        protected override void OnClosed(EventArgs e)
        {
            browser.Dispose();
            browser = null;
            base.OnClosed(e);
        }

        internal void LoadPage(string url)
        {
            if (!browser.IsBrowserInitialized)
            {
                _lastUrl = url;
                return;
            }
            Invoke(new Action(() =>
            {
                browser.Load(url);
            }));
        }
    }
}
