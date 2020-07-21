using DZY.WinAPI;
using Giantapp.LiveWallpaper.Engine.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Giantapp.LiveWallpaper.Engine.Renders
{
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
            string playerPath = $@"{appDir}\players\mpv\mpv.exe";
            string zipPath = $@"{appDir}\players\mpv.7z";

            //if (!File.Exists(playerPath) && File.Exists(zipPath))
            //{
            //    using ArchiveFile archiveFile = new ArchiveFile(zipPath);
            //    archiveFile.Extract($@"{appDir}\players\mpv");
            //}

            playerPath = @"D:\gitee\LiveWallpaperEngine\LiveWallpaperEngineWebRender\bin\x86\Debug\netcoreapp3.1\LiveWallpaperEngineWebRender.exe";
            if (!File.Exists(playerPath))
                return null;

            var r = new ProcessStartInfo(playerPath);

            r.Arguments = path;// $"\"{path}\" --hwdec=auto --panscan=1.0 --loop-file=inf --fs";
            r.UseShellExecute = false;
            return r;
        }
    }
}
