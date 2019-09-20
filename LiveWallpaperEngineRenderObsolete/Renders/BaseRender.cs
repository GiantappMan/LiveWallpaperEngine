using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveWallpaperEngineRender.Renders
{
    public class BaseRender
    {
        public bool RenderDisposed { protected set; get; }

        public bool Paused { protected set; get; }

        public bool Playing { protected set; get; }

        public string CurrentPath { protected set; get; }
    }
}
