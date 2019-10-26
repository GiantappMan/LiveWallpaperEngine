using System;
using System.Collections.Generic;
using System.Text;

namespace LiveWallpaperEngineAPI.Models
{
    public class ScreenOption
    {
        public ActionWhenMaximized WhenAppMaximized { get; set; }
        public uint ScreenIndex { get; internal set; }
    }
}
