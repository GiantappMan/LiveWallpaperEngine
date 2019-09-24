using LiveWallpaperEngine.Common;
using LiveWallpaperEngine.Wallpaper.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace LiveWallpaperEngineRender.Renders
{
    public interface IRender
    {
        List<WallpaperType.DefinedType> SupportTypes { get; }
        IntPtr Handle { get; }

        void Dispose();
        void Show(LaunchWallpaper data, Action<IntPtr> callBack);
    }
}
