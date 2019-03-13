using LiveWallpaperEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LiveWallpaperEngineRender.Renders
{
    public class HandleRender : BaseRender, IRender
    {
        IntPtr _handle;
        LiveWallpaperEngineCore _core;
        public HandleRender()
        {
        }

        public void SetCore(LiveWallpaperEngineCore core)
        {
            _core = core;
        }

        public void SetHandle(IntPtr handle)
        {
            _handle = handle;
        }

        public void CloseRender()
        {
            _core?.RestoreParent();
        }

        public IntPtr RestartRender()
        {
            return _handle;
        }

        public IntPtr ShowRender()
        {
            return _handle;
        }
    }
}
