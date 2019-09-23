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
        public Browser()
        {
            WebView web = new WebView();
            web.Source = new Uri("https://www.baidu.com");
            web.Dock = DockStyle.Fill;
            InitializeComponent();
            Controls.Add(web);
        }
    }
}
