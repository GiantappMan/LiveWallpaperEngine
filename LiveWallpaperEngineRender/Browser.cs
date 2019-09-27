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
            //Monitor parent process exit and close subprocesses if parent process exits first
            //This will at some point in the future becomes the default
            CefSharpSettings.SubprocessExitIfParentProcessClosed = true;

            //For Windows 7 and above, best to include relevant app.manifest entries as well
            Cef.EnableHighDPISupport();

            var settings = new CefSettings()
            {
                //By default CefSharp will use an in-memory cache, you need to specify a Cache Folder to persist data
                CachePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "CefSharp\\Cache")
            };

            //Example of setting a command line argument
            //Enables WebRTC
            settings.CefCommandLineArgs.Add("enable-media-stream", "1");

            //Perform dependency check to make sure all relevant resources are in our output directory.
            Cef.Initialize(settings, performDependencyCheck: true, browserProcessHandler: null);

        }


        //WebView web = new WebView();
        private readonly ChromiumWebBrowser browser;
        public Browser()
        {
            //web.IsPrivateNetworkClientServerCapabilityEnabled = true;
            //web.Source = new Uri(@"file:///F:/work/gitee/LiveWallpaperEngine/LiveWallpaperEngine.Samples.NetCore.Test/WallpaperSamples/web.html
            //", UriKind.RelativeOrAbsolute);
            //web.Source = new Uri("https://www.baidu.com");
            //web.Dock = DockStyle.Fill;
            InitializeComponent();
            browser = new ChromiumWebBrowser("www.google.com")
            {
                Dock = DockStyle.Fill,
            };
            Controls.Add(browser);
        }
        protected override void OnClosed(EventArgs e)
        {
            //web.Dispose();
            //web = null;
            base.OnClosed(e);
        }

        internal void LoadPage(string path)
        {
            //web.Source = new Uri(path);
            var url = new Uri(path, UriKind.RelativeOrAbsolute);
            //web.NavigateToLocalStreamUri(url, new StreamUriResolver());
        }
    }
}
