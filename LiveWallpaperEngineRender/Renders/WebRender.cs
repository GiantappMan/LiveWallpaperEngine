using System;
using System.Collections.Generic;
using LiveWallpaperEngine.Common;
using LiveWallpaperEngine.Common.Models;

namespace LiveWallpaperEngineRender.Renders
{
    class WebRender : IRender
    {
        Browser browserForm;
        public WebRender()
        {
        }

        public List<WallpaperType.DefinedType> SupportTypes => new List<WallpaperType.DefinedType>()
        { WallpaperType.DefinedType.Web };
        public IntPtr Handle { private set; get; }

        public void Dispose()
        {
            browserForm.Close();
            browserForm = null;
        }

        public void Show(LaunchWallpaper data, Action<IntPtr> callBack)
        {
            if (browserForm == null)
            {
                browserForm = new Browser();
                //mpvForm.Load += MpvForm_Load;
                browserForm.Show();
            }
            browserForm.LoadPage(data.Wallpaper.Path);
            callBack?.Invoke(browserForm.Handle);
        }
    }
}
