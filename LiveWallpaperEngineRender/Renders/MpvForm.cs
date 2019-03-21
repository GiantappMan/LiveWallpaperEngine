using LiveWallpaperEngine;
using Mpv.NET.Player;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LiveWallpaperEngineRender.Renders
{
    public partial class MpvForm : Form
    {
        public MpvPlayer Player { get; set; }

        #region construct

        public MpvForm()
        {
            //UI
            BackColor = Color.Magenta;
            TransparencyKey = Color.Magenta;
            ShowInTaskbar = false;
            InitializeComponent();
            FormBorderStyle = FormBorderStyle.None;

            //callback
            FormClosing += RenderForm_FormClosing;
        }


        private void RenderForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            FormClosing -= RenderForm_FormClosing;
            Player?.Dispose();
            Player = null;
        }

        #endregion
    }
}
