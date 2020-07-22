using SevenZipExtractor;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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

        protected override ProcessStartInfo GenerateProcessInfo(string path)
        {
            //文档：https://mpv.io/manual/stable/
            var assembly = Assembly.GetEntryAssembly();
            string appDir = Path.GetDirectoryName(assembly.Location);
            string playerPath = $@"{appDir}\players\web\LiveWallpaperEngineWebRender.exe";
            string zipPath = $@"{appDir}\players\web.7z";

            if (!File.Exists(playerPath) && File.Exists(zipPath))
            {
                using ArchiveFile archiveFile = new ArchiveFile(zipPath);
                archiveFile.Extract($@"{appDir}\players\web");
            }

            playerPath = @"D:\gitee\LiveWallpaperEngine\LiveWallpaperEngineWebRender\bin\x86\Debug\netcoreapp3.1\LiveWallpaperEngineWebRender.exe";
            if (!File.Exists(playerPath))
                return null;

            var r = new ProcessStartInfo(playerPath);

            //r.Arguments = $"\"{path}\" --position=-10000,-10000";
            r.Arguments = $"\"{path}\" --position=-0,0";
            r.UseShellExecute = false;
            return r;
        }

        protected override async Task<(IntPtr Handle, int PId)> StartProcess(string path, CancellationToken ct)
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
                 var handle = title.Substring(index + 4);
                 result.Handle = new IntPtr(int.Parse(handle));
                 return result;
             });
        }
    }
}
