using System;
using System.Collections.Generic;
using System.Text;

namespace Giantapp.LiveWallpaper.Engine.Renders
{
    public class ExeRender : ExternalRender
    {
        public ExeRender() : base(WallpaperType.Exe, new List<string>() { ".exe" })
        {

        }
    }
}
