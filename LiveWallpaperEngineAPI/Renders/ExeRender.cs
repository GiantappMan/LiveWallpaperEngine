using Giantapp.LiveWallpaper.Engine.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Giantapp.LiveWallpaper.Engine.Renders
{
    public class ExeRender : ExternalProcessRender
    {
        public ExeRender() : base(WallpaperType.Exe, new List<string>() { ".exe" })
        {

        }
    }
}
