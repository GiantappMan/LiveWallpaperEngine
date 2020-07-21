using CefSharp.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LiveWallpaperEngineWebRender
{
    public partial class BrowserForm : Form
    {
        private static List<BrowserForm> _allForms = new List<BrowserForm>();

        private readonly ChromiumWebBrowser browser;
        public BrowserForm(string url)
        {
            InitializeComponent();

            Location = new Point(-10000, -10000);
            Width = Screen.PrimaryScreen.Bounds.Width;
            Height = Screen.PrimaryScreen.Bounds.Height;
            FormBorderStyle = FormBorderStyle.None;
            StartPosition = FormStartPosition.Manual;

            browser = new ChromiumWebBrowser(url);
            Text = $"WebRender {url}";
            Controls.Add(browser);

            _allForms.Add(this);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            _allForms.Remove(this);
            base.OnClosing(e);
            if (_allForms.Count == 0)
                Application.Exit();
        }
    }
}
