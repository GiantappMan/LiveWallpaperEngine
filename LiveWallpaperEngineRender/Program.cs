using LiveWallpaperEngine.Renders;
using LiveWallpaperEngine.Wallpaper.Models;
using MpvPlayer;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace LiveWallpaperEngineRender
{
    static class Program
    {
        private static IPCHelper ipc = new IPCHelper();
        private static string path;
        private static WallpaperType.DefinedType type;
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            //System.Windows.MessageBox.Show("1");

            var parent = Process.GetCurrentProcess().Parent();
            if (parent != null)
            {
                //父进程退出自动退出
                parent.EnableRaisingEvents = true;
                parent.Exited += Parent_Exited;
            }

            type = Enum.Parse<WallpaperType.DefinedType>(args[0]);
            path = args[1];

            switch (type)
            {
                case WallpaperType.DefinedType.Video:
                    MpvForm form = new MpvForm();
                    form.Load += Form_Load;
                    form.InitPlayer();
                    System.Windows.Forms.Application.Run(form);

                    break;
            }

            //Application.SetHighDpiMode(HighDpiMode.SystemAware);
            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new Form1());            
        }

        private static void Parent_Exited(object sender, EventArgs e)
        {
            Process.GetCurrentProcess().Kill();
        }

        private static void Form_Load(object sender, EventArgs e)
        {
            MpvForm form = sender as MpvForm;
            form.Player.AutoPlay = true;
            form.Player.Load(path);

            ipc.Send(new RenderInitlized()
            {
                Handle = form.Handle
            });
        }
    }
}
