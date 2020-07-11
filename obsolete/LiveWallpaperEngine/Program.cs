using System;
using System.Windows.Forms;
using LiveWallpaperEngineAPI.Common;
using LiveWallpaperEngineAPI.Renders;
using LiveWallpaperEngineRemoteWebRender;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Hosting;

namespace LiveWallpaperEngine
{
    public static class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            CreateHostBuilder(args)
                .Build()
                .RunAsync();

            //winform设置
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            //异常捕获
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.ThreadException += Application_ThreadException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            //dpi 相关
            //上面已经操作了不用设了
            //User32WrapperEx.SetThreadAwarenessContext(DPI_AWARENESS_CONTEXT.DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE);
            WallpaperHelper.DoSomeMagic();

            //用node+electron+http api渲染，待c#有更好的库时，再考虑c#渲染
            RenderFactory.Renders.Add(typeof(ElectronWebRender), ElectronWebRender.StaticSupportTypes);

            //winform 处理消息循环
            Application.Run(new Form()
            {
                ShowInTaskbar = false,
                Opacity = 0
            });

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
        }

        // Additional configuration is required to successfully run gRPC on macOS.
        // For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder
                    .ConfigureKestrel(options =>
                      {
                          // Setup a HTTP/2 endpoint without TLS.
                          int port = 8080;
                          if (args.Length > 0)
                              port = int.Parse(args[0]);
                          options.ListenLocalhost(port, o => o.Protocols =
                              HttpProtocols.Http2);
                      })
                    .UseStartup<Startup>();
                    //.UseUrls("http://localhost:9090");
                });
    }
}
