using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LiveWallpaperEngine.Common.Models;
using LiveWallpaperEngine.Common.Renders;
using LiveWallpaperEngineRender.Forms;

namespace LiveWallpaperEngineRender.Renders
{
    class VideoRender : IRender
    {
        readonly Dictionary<int, VideoControl> _controls = new Dictionary<int, VideoControl>();

        public static List<WallpaperType> StaticSupportTypes => new List<WallpaperType>()
        {
            new VideoWallpaperType(),
            new ImageWallpaperType()
        };

        List<WallpaperType> IRender.SupportTypes => StaticSupportTypes;

        public void CloseWallpaper(params int[] screenIndexs)
        {
            foreach (var index in screenIndexs)
            {
                _controls[index].Stop();
                var screen = RenderHost.GetHost(index);
                screen.RemoveWallpaper(_controls[index]);
            }
        }

        public void Dispose()
        {
            foreach (var item in _controls)
            {
                item.Value.DiposePlayer();
                item.Value.Dispose();
            }
            _controls.Clear();
        }

        public int GetVolume(params int[] screenIndexs)
        {
            throw new NotImplementedException();
        }

        public void Pause(params int[] screenIndexs)
        {
            foreach (var index in screenIndexs)
            {
                _controls[index].Pause();
            }
        }

        public void Resum(params int[] screenIndexs)
        {
            foreach (var index in screenIndexs)
            {
                _controls[index].Resum();
            }
        }

        public void SetVolume(int v, params int[] screenIndexs)
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
                        _controls[index] = new VideoControl();
                        _controls[index].InitPlayer();
                    });
                }
                var screen = RenderHost.GetHost(index);
                screen.ShowWallpaper(_controls[index], this);
                _controls[index].Load(wallpaper.Path);
            }
            return Task.CompletedTask;
        }
    }
}
