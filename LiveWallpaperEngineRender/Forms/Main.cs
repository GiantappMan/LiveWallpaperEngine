using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace LiveWallpaperEngineRender
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
            WindowState = FormWindowState.Maximized;
            ShowInTaskbar = false;
            Opacity = 0;
        }


        public static void UIInvoke(Action a)
        {
            var mainForm = Application.OpenForms[0];
            if (mainForm == null)
                return;

            if (mainForm.InvokeRequired)
                mainForm.Invoke(a);
            else
                a();
        }
    }
}
