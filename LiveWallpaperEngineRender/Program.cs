using LiveWallpaperEngine.Common;
using LiveWallpaperEngine.Common.Renders;
using LiveWallpaperEngineRender.Renders;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LiveWallpaperEngineRender
{
    static class Program
    {
        static IPCHelper _ipc = null;
        static Form _mainForm;

        [STAThread]
        static void Main(string[] args)
        {
            //注册render
            RenderFactory.Renders.Add(typeof(VideoRender), VideoRender.StaticSupportTypes);
            RenderFactory.Renders.Add(typeof(WebRender), WebRender.StaticSupportTypes);

            WatchParent();

            _ipc = new IPCHelper(IPCHelper.RemoteRenderID, IPCHelper.ServerID);
            _ipc.MsgReceived += Ipc_MsgReceived;

            //winform设置
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            //异常捕获
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.ThreadException += Application_ThreadException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            _mainForm = new RenderHost(0);
            _mainForm.Load += Main_Load;
            Application.Run(_mainForm);
        }

        static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            var error = e.Exception;
            MessageBox.Show(error?.Message, "ThreadException");
            Environment.Exit(0);
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var error = e.ExceptionObject as Exception;
            MessageBox.Show(error?.Message, "UnhandledException");
            Environment.Exit(0);
        }

        private static void Main_Load(object sender, EventArgs e)
        {
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
            if (e.CommandFullName == typeof(InvokeRender).FullName)
            {
                var data = JsonConvert.DeserializeObject<InvokeRender>(e.Parameter);

                var currentRender = RenderFactory.GetOrCreateRender(data.DType);
                var method = currentRender.GetType().GetMethod(data.InvokeMethod);
                var methodParameters = method.GetParameters();

                var parameters = new object[methodParameters.Length];
                for (int i = 0; i < methodParameters.Length; i++)
                    parameters[i] = JsonConvert.DeserializeObject(data.Parameters[i].ToString(), methodParameters[i].ParameterType);
                //转发到本地IRender
                method.Invoke(currentRender, parameters);
            }
        }

        private static void Parent_Exited(object sender, EventArgs e)
        {
            Process.GetCurrentProcess().Kill();
        }
    }
}
