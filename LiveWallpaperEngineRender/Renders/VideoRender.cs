using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;
using LiveWallpaperEngine.Common;
using LiveWallpaperEngine.Common.Models;
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
            Main.UIInvoke(() =>
            {
                videoForm.Close();
            });
        }

        public void Show(LaunchWallpaper data, Action<IntPtr> callBack)
        {
            if (videoForm == null)
            {
                Main.UIInvoke(() =>
                {
                    videoForm = new Video();
                    videoForm.Show();
                    WallpaperHelper.GetInstance(data.ScreenIndex).SendToBackground(videoForm.Handle);
                });
            }
            videoForm.LoadFile(data.Wallpaper.Path);
            callBack?.Invoke(videoForm.Handle);
        }
    }
}
