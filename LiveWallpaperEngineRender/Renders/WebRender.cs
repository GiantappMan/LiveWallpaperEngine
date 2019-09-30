using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LiveWallpaperEngine.Common.Models;
using LiveWallpaperEngine.Common.Renders;

namespace LiveWallpaperEngineRender.Renders
{
    class WebRender : IRender
    {
        Browser browserForm;
        public WebRender()
        {
        }

        public static List<WallpaperType> StaticSupportTypes => new List<WallpaperType>()
        {
            new WebWallpaperType(),
        };
        public List<WallpaperType> SupportTypes => StaticSupportTypes;

        public IntPtr Handle { private set; get; }


        public void CloseWallpaper(params int[] screenIndexs)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            RenderHost.UIInvoke(() =>
            {
                browserForm.Close();
                browserForm = null;
            });
        }

        public int GetVolume()
        {
            throw new NotImplementedException();
        }

        public void Pause()
        {
            throw new NotImplementedException();
        }

        public void Resum()
        {
            throw new NotImplementedException();
        }

        public void SetVolume(int v)
        {
            throw new NotImplementedException();
        }

        public Task ShowWallpaper(WallpaperModel wallpaper, params int[] screenIndex)
        {
            if (browserForm == null)
            {
                RenderHost.UIInvoke(() =>
                {
                    browserForm = new Browser();
                    browserForm.Show();
                    //WallpaperHelper.GetInstance(data.ScreenIndexs).SendToBackground(browserForm.Handle);
                });
            }
            browserForm.LoadPage(wallpaper.Path);
            return Task.CompletedTask;
        }
    }
}
