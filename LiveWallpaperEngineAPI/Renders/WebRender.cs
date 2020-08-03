using DZY.WinAPI;
using Giantapp.LiveWallpaper.Engine.Utils;
using SharpCompress.Archives;
using SharpCompress.Archives.SevenZip;
using SharpCompress.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Giantapp.LiveWallpaper.Engine.Renders
{
    //本想单进程渲染多屏幕，但是发现声音不可控。故改为单进程只渲染一个屏幕
    public class WebRender : ExternalProcessRender
    {
        public WebRender() : base(WallpaperType.Web, new List<string>() { ".html", ".htm" })
        {

        }

        protected override ProcessStartInfo GetRenderExeInfo(string path)
        {
            //文档：https://mpv.io/manual/stable/
            var assembly = Assembly.GetEntryAssembly();
            string appDir = Path.GetDirectoryName(assembly.Location);
            string playerPath = $@"{appDir}\players\web\LiveWallpaperEngineWebRender.exe";
            string zipPath = $@"{appDir}\players\web.7z";

            if (!File.Exists(playerPath) && File.Exists(zipPath))
            {
                SevenZip archiveFile = new SevenZip(zipPath);
                archiveFile.Extract($@"{appDir}\players\web");
            }

            if (!File.Exists(playerPath))
                return null;

            var r = new ProcessStartInfo(playerPath)
            {
                Arguments = $"\"{path}\" --position=-0,0",
                UseShellExecute = false
            };
            return r;
        }

        protected override async Task<RenderProcess> StartProcess(string path, CancellationToken ct)
        {
            var result = await base.StartProcess(path, ct);
            return await Task.Run(() =>
             {
                 var p = Process.GetProcessById(result.PId);
                 string title = p.MainWindowTitle;

                 var index = title.IndexOf("cef=");

                 while (index < 0)
                 {
                     p?.Dispose();
                     p = Process.GetProcessById(result.PId);

                     title = p.MainWindowTitle;
                     index = title.IndexOf("cef=");
                     Thread.Sleep(10);
                 }
                 p?.Dispose();
                 var handleStr = title.Substring(index + 4);

                 var cefHandle = new IntPtr(int.Parse(handleStr));
                 var handle = User32Wrapper.FindWindowEx(cefHandle, IntPtr.Zero, "Chrome_WidgetWin_0", IntPtr.Zero);

                 result.ReceiveMouseEventHandle = handle;
                 return result;
             });
        }
    }
}
