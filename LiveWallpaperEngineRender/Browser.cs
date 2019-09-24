using Microsoft.Toolkit.Forms.UI.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace LiveWallpaperEngineRender
{
    public partial class Browser : Form
    {
        WebView web = new WebView();
        public Browser()
        {
            web.IsPrivateNetworkClientServerCapabilityEnabled = true;
            web.Source = new Uri(@"file:///F:/work/gitee/LiveWallpaperEngine/LiveWallpaperEngine.Samples.NetCore.Test/WallpaperSamples/web.html", UriKind.RelativeOrAbsolute);
            //web.Source = new Uri("https://www.baidu.com");
            web.Dock = DockStyle.Fill;
            InitializeComponent();
            Controls.Add(web);

        }
        protected override void OnClosed(EventArgs e)
        {
            web.Dispose();
            web = null;
            base.OnClosed(e);
        }

        internal void LoadPage(string path)
        {
            //web.Source = new Uri(path);
        }
    }
}
