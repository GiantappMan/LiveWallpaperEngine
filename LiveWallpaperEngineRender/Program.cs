using LiveWallpaperEngine.Common;
using LiveWallpaperEngine.Common.Models;
using LiveWallpaperEngine.Common.Renders;
using LiveWallpaperEngineRender.Renders;
using Newtonsoft.Json;
using System;
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
        static void Main()
        {
            WallpaperManager.Initlize();
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
            if (e.CommandFullName == typeof(InvokeILiveWallpaperAPI).FullName)
            {
                var data = JsonConvert.DeserializeObject<InvokeILiveWallpaperAPI>(e.Parameter);
                AssignToTarget(WallpaperManager.Instance, data.InvokeMethod, data.Parameters);
            }
        }

        //把ipc远程对象转发到本地对象
        private static void AssignToTarget(object target, string invokeMethod, object[] jsonParameters)
        {
            var method = target.GetType().GetMethod(invokeMethod);
            var methodParameters = method.GetParameters();
            var parameters = new object[methodParameters.Length];

            for (int i = 0; i < methodParameters.Length; i++)
                parameters[i] = JsonConvert.DeserializeObject(jsonParameters[i].ToString(), methodParameters[i].ParameterType);
            method.Invoke(target, parameters);
        }

        private static void Parent_Exited(object sender, EventArgs e)
        {
            Process.GetCurrentProcess().Kill();
        }
    }
}
