using DZY.WinAPI;
using LiveWallpaperEngine.Common;
using LiveWallpaperEngine.Wallpaper.Models;
using LiveWallpaperEngineRender.Renders;
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

            //string screenIndex = "0";
            //if (args.Length > 0)
            //    screenIndex = args[0];

            _ipc = new IPCHelper(IPCHelper.RemoteRenderID, IPCHelper.ServerID);
            _ipc.MsgReceived += Ipc_MsgReceived;

            wf.Application.SetHighDpiMode(wf.HighDpiMode.SystemAware);
            wf.Application.EnableVisualStyles();
            wf.Application.SetCompatibleTextRenderingDefault(false);

            //处理未捕获的异常   
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            //处理UI线程异常   
            Application.ThreadException += Application_ThreadException;
            //处理非UI线程异常   
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            _mainForm = new Main();
            _mainForm.Load += Main_Load;
            _mainForm.Hide();
            wf.Application.Run(_mainForm);
        }
        static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            string str;
            var strDateInfo = "出现应用程序未处理的异常：" + DateTime.Now + "\r\n";
            var error = e.Exception;
            if (error != null)
            {
                str = string.Format(strDateInfo + "异常类型：{0}\r\n异常消息：{1}\r\n异常信息：{2}\r\n",
                    error.GetType().Name, error.Message, error.StackTrace);
            }
            else
            {
                str = string.Format("应用程序线程错误:{0}", e);
            }

            MessageBox.Show("发生错误，请查看程序日志！", "系统错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            Environment.Exit(0);
        }


        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var error = e.ExceptionObject as Exception;
            var strDateInfo = "出现应用程序未处理的异常：" + DateTime.Now + "\r\n";
            var str = error != null ? string.Format(strDateInfo + "Application UnhandledException:{0};\n\r堆栈信息:{1}", error.Message, error.StackTrace) : string.Format("Application UnhandledError:{0}", e);


            MessageBox.Show("发生错误，请查看程序日志！", "系统错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            Environment.Exit(0);
        }

        private static void Main_Load(object sender, EventArgs e)
        {
            //_mainForm.Invoke(new Action(() =>
            //{
            //    //test
            //    WebRender w = new WebRender();
            //    w.Show(new LaunchWallpaper()
            //    {
            //        Path = @"F:\work\gitee\LiveWallpaperEngine\LiveWallpaperEngine.Samples.NetCore.Test\WallpaperSamples\html\index.html"
            //    }, null);
            //}));
            //_mainForm.ShowInTaskbar = false;
            //_mainForm.Opacity = 0;
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
                if (_currentRender == null || !_currentRender.SupportTypes.Contains(data.Wallpaper.Type.DType))
                {
                    _currentRender?.Dispose();
                    _currentRender = _allRenders.FirstOrDefault(m => m.SupportTypes.Contains(data.Wallpaper.Type.DType));
                }

                _mainForm.BeginInvoke(new Action(() =>
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
