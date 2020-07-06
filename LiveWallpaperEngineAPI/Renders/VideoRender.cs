using System;
using System.Collections.Generic;
using System.Text;

namespace Giantapp.LiveWallpaper.Engine.Renders
{
    public class VideoRender : ExternalProcessRender
    {
        public VideoRender() : base(WallpaperType.Video, new List<string>() { ".mp4", ".flv", ".blv", ".avi", ".gif", })
        {

        }
    }
}
