using LiveWallpaperEngine.Renders;
using LiveWallpaperEngine.Wallpaper.Models;
using MpvPlayer;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace LiveWallpaperEngineRender
{
    static class Program
    {
        private static IPCHelper _ipc = null;
        private static DateTime test;
        //private static string _serverIpcID = null;
        //private static string _handshakeID = null;

        private static MpvForm _videoForm = null;

        private static string path;
        private static WallpaperType.DefinedType type;
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            test = DateTime.Now;
            //System.Windows.MessageBox.Show("1");

            var parent = Process.GetCurrentProcess().Parent();
            if (parent != null)
            {
                //父进程退出自动退出
                parent.EnableRaisingEvents = true;
                parent.Exited += Parent_Exited;
            }

            string serverIpcId = args[0];
            string clientIpcId = args[1];

            _ipc = new IPCHelper(clientIpcId, serverIpcId);
            _ipc.MsgReceived += Ipc_MsgReceived;
            //type = Enum.Parse<WallpaperType.DefinedType>(args[0]);
            //path = args[1];

            switch (type)
            {
                case WallpaperType.DefinedType.Video:
                    _videoForm = new MpvForm();
                    _videoForm.InitPlayer();
                    _videoForm.Player.AutoPlay = true;
                    _videoForm.Load += _videoForm_Load;
                    System.Windows.Forms.Application.Run(_videoForm);
                    break;
            }

            //Application.SetHighDpiMode(HighDpiMode.SystemAware);
            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new Form1());            
        }

        private static void _videoForm_Load(object sender, EventArgs e)
        {
            System.Windows.Forms.MessageBox.Show((DateTime.Now - test).ToString());
            _ipc.Send(new RenderInitlized()
            {
                Handle = _videoForm.Handle
            });
        }

        private static void Ipc_MsgReceived(object sender, Command e)
        {
            if (e.CommandFullName == typeof(LaunchWallpaper).FullName)
            {
                var dataLaunchWallpaper = JsonConvert.DeserializeObject<LaunchWallpaper>(e.Parameter);
                if (dataLaunchWallpaper != null)
                {
                    //_videoForm.Player.Stop();
                    _videoForm.Player.Pause();
                    _videoForm.Player.Load(dataLaunchWallpaper.Path);
                    _videoForm.Player.Resume();
                }
            }
        }

        private static void Parent_Exited(object sender, EventArgs e)
        {
            Process.GetCurrentProcess().Kill();
        }
    }
}
