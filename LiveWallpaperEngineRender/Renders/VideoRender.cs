using System;
using System.Collections.Generic;
using LiveWallpaperEngine.Common;
using LiveWallpaperEngine.Wallpaper.Models;
using LiveWallpaperEngineRender.Forms;

namespace LiveWallpaperEngineRender.Renders
{
    class VideoRender : IRender
    {
        Video videoForm;
        public List<WallpaperType.DefinedType> SupportTypes => new List<WallpaperType.DefinedType>() {
                WallpaperType.DefinedType.Video,
                WallpaperType.DefinedType.Image
        };

        public IntPtr Handle { private set; get; }

        public void Dispose()
        {
            videoForm.DiposePlayer();
            videoForm.Close();
        }

        public void Show(LaunchWallpaper data, Action<IntPtr> callBack)
        {
            if (videoForm == null)
            {
                videoForm = new Video();
                videoForm.Show();
            }
            videoForm.LoadFile(data.Wallpaper.Path);
            callBack?.Invoke(videoForm.Handle);
        }
    }
}
