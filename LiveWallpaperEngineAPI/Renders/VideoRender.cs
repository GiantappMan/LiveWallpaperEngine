using Giantapp.LiveWallpaper.Engine.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Reflection;

namespace Giantapp.LiveWallpaper.Engine.Renders
{
    public class VideoRender : ExternalProcessRender
    {
        public VideoRender() : base(WallpaperType.Video, new List<string>() { ".mp4", ".flv", ".blv", ".avi", ".gif", }, false)
        {

        }

        protected override ProcessStartInfo GetRenderExeInfo(string path)
        {
            //文档：https://mpv.io/manual/stable/
            var assembly = Assembly.GetEntryAssembly();
            string appDir = Path.GetDirectoryName(assembly.Location);
            string playerPath = $@"{appDir}\players\mpv\mpv.exe";
            string zipPath = $@"{appDir}\players\mpv.7z";

            if (!File.Exists(playerPath) && File.Exists(zipPath))
            {
                SevenZip archiveFile = new SevenZip(zipPath);
                archiveFile.Extract($@"{appDir}\players\mpv");
            }

            if (!File.Exists(playerPath))
                return null;

            ProcessStartInfo r = new ProcessStartInfo(playerPath)
            {
                Arguments = $"\"{path}\" --hwdec=auto --panscan=1.0 --loop-file=inf --fs --geometry=-10000:-10000",
                UseShellExecute = false
            };
            return r;
        }
    }
}
