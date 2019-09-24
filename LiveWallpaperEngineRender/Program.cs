using DZY.WinAPI;
using LiveWallpaperEngine.Common;
using MpvPlayer;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using wf = System.Windows.Forms;

namespace LiveWallpaperEngineRender
{
    static class Program
    {
        internal static IPCHelper _ipc = null;

        private static MpvForm _videoForm = null;

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            //System.Windows.MessageBox.Show("1");
            //隐藏控制台
            var handle = Kernel32Wrapper.GetConsoleWindow();
            User32Wrapper.ShowWindow(handle, WINDOWPLACEMENTFlags.SW_HIDE);

            WatchParent();

            string serverIpcId = args.Length > 0 ? args[0] : "serverIpc";
            string clientIpcId = args.Length > 1 ? args[1] : "clientIpc";

            _ipc = new IPCHelper(clientIpcId, serverIpcId);
            _ipc.MsgReceived += Ipc_MsgReceived;

            //_videoForm = new MpvForm();
            //_videoForm.InitPlayer();
            //_videoForm.Player.AutoPlay = true;
            //_videoForm.Load += _videoForm_Load;
            //System.Windows.Forms.Application.Run(_videoForm);

            wf.Application.SetHighDpiMode(wf.HighDpiMode.SystemAware);
            wf.Application.EnableVisualStyles();
            wf.Application.SetCompatibleTextRenderingDefault(false);
            //wf.Application.Run(new Main());
            wf.Application.Run(new Browser());
            Console.ReadLine();
        }

        private static void WatchParent()
        {
            Task.Run(() =>
            {
                var parent = Process.GetCurrentProcess().Parent();
                if (parent != null)
                {
                    //父进程退出自动退出
                    parent.EnableRaisingEvents = true;
                    parent.Exited += Parent_Exited;
                }
            });
        }

        private static void _videoForm_Load(object sender, EventArgs e)
        {
            //System.Windows.Forms.MessageBox.Show((DateTime.Now - test).ToString());
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
