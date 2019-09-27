using DZY.WinAPI;
using LiveWallpaperEngine.Common;
using LiveWallpaperEngine.Wallpaper.Models;
using LiveWallpaperEngineRender.Renders;
using MpvPlayer;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using wf = System.Windows.Forms;

namespace LiveWallpaperEngineRender
{
    static class Program
    {
        static IPCHelper _ipc = null;
        static IRender _currentRender = null;
        static List<IRender> _allRenders = new List<IRender>()
        {
            new WebRender(),
            new VideoRender()
        };
        static Form _mainForm;
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            //System.Windows.MessageBox.Show(args[0]);
            ////隐藏控制台
            //var handle = Kernel32Wrapper.GetConsoleWindow();
            //User32Wrapper.ShowWindow(handle, WINDOWPLACEMENTFlags.SW_HIDE);

            WatchParent();

            string screenIndex = "0";
            if (args.Length > 0)
                screenIndex = args[0];

            _ipc = new IPCHelper(IPCHelper.RemoteRenderID + screenIndex, IPCHelper.ServerID + screenIndex);
            _ipc.MsgReceived += Ipc_MsgReceived;

            wf.Application.SetHighDpiMode(wf.HighDpiMode.SystemAware);
            wf.Application.EnableVisualStyles();
            wf.Application.SetCompatibleTextRenderingDefault(false);
   
            _mainForm = new Main();
            _mainForm.Load += Main_Load;
            _mainForm.Hide();
            wf.Application.Run(_mainForm);
            Console.ReadLine();
        }

        private static void Main_Load(object sender, EventArgs e)
        {
            _mainForm.Invoke(new Action(() =>
            {
                //var browser = new CefSharp.MinimalExample.WinForms.BrowserForm();
                //browser.Show();
                //var b = new Browser();
                //b.Show();

                //test
                WebRender w = new WebRender();
                w.Show(new LaunchWallpaper()
                {
                    Path = @"lvp:///work\gitee\LiveWallpaperEngine\LiveWallpaperEngine.Samples.NetCore.Test\WallpaperSamples\web.html"
                }, null);

                WebRender w2 = new WebRender();
                w2.Show(new LaunchWallpaper()
                {
                    Path = @"lvp:///work\gitee\LiveWallpaperEngine\LiveWallpaperEngine.Samples.NetCore.Test\WallpaperSamples\web.html"
                }, null);
            }));
            _mainForm.Hide();
            _mainForm.Load -= Main_Load;
            _ipc.Send(new Ready());
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

        private static void Ipc_MsgReceived(object sender, Command e)
        {
            if (e.CommandFullName == typeof(LaunchWallpaper).FullName)
            {
                var data = JsonConvert.DeserializeObject<LaunchWallpaper>(e.Parameter);
                if (_currentRender == null || !_currentRender.SupportTypes.Contains(data.Type))
                {
                    _currentRender?.Dispose();
                    _currentRender = _allRenders.FirstOrDefault(m => m.SupportTypes.Contains(data.Type));
                }

                _mainForm.Invoke(new Action(() =>
                {
                    _currentRender.Show(data, SendHandle);
                }));
                //switch (data.Type)
                //{
                //    case WallpaperType.DefinedType.Image:
                //    case WallpaperType.DefinedType.Video:
                //        break;
                //    case WallpaperType.DefinedType.Web:
                //        var videoForm = new MpvForm();
                //        videoForm.InitPlayer();
                //        videoForm.Player.AutoPlay = true;
                //        SendHandle();
                //        break;
                //}
                //if (_currentRender is MpvForm)
                //{
                //    if (data != null)
                //    {
                //        //_videoForm.Player.Stop();
                //        videoForm.Player.Pause();
                //        videoForm.Player.Load(data.Path);
                //        videoForm.Player.Resume();
                //    }
                //}
            }
        }

        private static void SendHandle(IntPtr handle)
        {
            _ipc.Send(new LaunchWallpaperResult()
            {
                Handle = handle
            });
        }

        private static void Parent_Exited(object sender, EventArgs e)
        {
            Process.GetCurrentProcess().Kill();
        }
    }
}
