using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TestEmbedWindow
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            Load += Form1_Load;

            //UI
            //BackColor = Color.Magenta;
            //TransparencyKey = Color.Magenta;
            //ShowInTaskbar = false;
            //FormBorderStyle = FormBorderStyle.None;
            //WindowState = FormWindowState.Maximized;
            //Opacity = 0;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LoadApplication(@"D:\github-categorized\dotnet\LiveWallpaperEngine\LiveWallpaperEngine.Samples.NetCore.Test\WallpaperSamples\game\sheep.exe", this.Handle);
            //LoadApplication(@"c:\windows\system32\notepad.exe", this.Handle);
            Opacity = 1;
        }

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);
        [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
        public static extern IntPtr SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int x, int Y, int cx, int cy, int wFlags);

        private void LoadApplication(string path, IntPtr handle)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            int timeout = 10 * 1000;     // Timeout value (10s) in case we want to cancel the task if it's taking too long.

            ProcessStartInfo info = new ProcessStartInfo(path);
            info.WindowStyle = ProcessWindowStyle.Maximized;
            info.CreateNoWindow = true;
            Process p = Process.Start(info);
            while (p.MainWindowHandle == IntPtr.Zero)
            {
                System.Threading.Thread.Sleep(10);
                p.Refresh();

                if (sw.ElapsedMilliseconds > timeout)
                {
                    sw.Stop();
                    return;
                }
            }

            SetParent(p.MainWindowHandle, handle);      // Set the process parent window to the window we want
            SetWindowPos(p.MainWindowHandle, 0, 0, 0, 0, 0, 0x0001 | 0x0040);       // Place the window in the top left of the parent window without resizing it
        }
    }
}
