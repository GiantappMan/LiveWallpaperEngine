using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LiveWallpaperEngine.Common.Models;
using LiveWallpaperEngine.Common.Renders;
using LiveWallpaperEngineRender.Forms;

namespace LiveWallpaperEngineRender.Renders
{
    class WebRender : IRender
    {
        readonly Dictionary<int, WebControl> _controls = new Dictionary<int, WebControl>();
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
            foreach (var index in screenIndexs)
            {
                _controls[index].StopPage();
                var screen = RenderHost.GetHost(index);
                screen.RemoveWallpaper(_controls[index]);
            }
        }

        public void Dispose()
        {
            foreach (var item in _controls)
            {
                item.Value.DisposeWebBrowser();
                item.Value.Dispose();
            }
            _controls.Clear();
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

        public Task ShowWallpaper(WallpaperModel wallpaper, params int[] screenIndexs)
        {
            foreach (var index in screenIndexs)
            {
                if (!_controls.ContainsKey(index))
                {
                    RenderHost.UIInvoke(() =>
                    {
                        _controls[index] = new WebControl();
                    });
                }
                var screen = RenderHost.GetHost(index);
                screen.ShowWallpaper(_controls[index], this);
                _controls[index].LoadPage(wallpaper.Path);
            }
            return Task.CompletedTask;
        }
    }
}
