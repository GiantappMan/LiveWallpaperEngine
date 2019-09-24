using System;
using System.Collections.Generic;
using System.Text;
using LiveWallpaperEngine.Common;
using LiveWallpaperEngine.Wallpaper.Models;
using MpvPlayer;

namespace LiveWallpaperEngineRender.Renders
{
    class VideoRender : IRender
    {
        MpvForm mpvForm;
        public List<WallpaperType.DefinedType> SupportTypes => new List<WallpaperType.DefinedType>() {
                WallpaperType.DefinedType.Video,
                WallpaperType.DefinedType.Image
        };

        public IntPtr Handle { private set; get; }

        public void Dispose()
        {
            mpvForm.Player.Dispose();
            mpvForm.Close();
        }

        public void Show(LaunchWallpaper data, Action<IntPtr> callBack)
        {
            if (mpvForm == null)
            {
                mpvForm = new MpvForm();
                mpvForm.InitPlayer();
                mpvForm.Player.AutoPlay = true;
                //mpvForm.Load += MpvForm_Load;
                mpvForm.Show();
            }
            mpvForm.Player.Pause();
            mpvForm.Player.Load(data.Path);
            mpvForm.Player.Resume();
            callBack?.Invoke(mpvForm.Handle);
        }

        //private void MpvForm_Load(object sender, EventArgs e)
        //{
        //    mpvForm.Load -= MpvForm_Load;
        //    Handle = mpvForm.Handle;
        //    System.Windows.Forms.MessageBox.Show(mpvForm.Handle.ToString());
        //}
    }
}
