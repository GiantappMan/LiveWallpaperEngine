using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace LiveWallpaperEngine
{
    public interface IRender
    {
        bool RenderDisposed { get; }

        void SetCore(LiveWallpaperEngineCore core);

        IntPtr ShowRender();

        void CloseRender();

        IntPtr RestartRender();
    }
}
