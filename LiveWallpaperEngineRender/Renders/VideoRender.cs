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
        readonly Dictionary<int, VideoControl> _videoControls = new Dictionary<int, VideoControl>();

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
                _videoControls[index].Stop();
                var screen = RenderHost.GetHost(index);
                screen.RemoveWallpaper(_videoControls[index]);
            }
        }

        public void Dispose()
        {
            foreach (var item in _videoControls)
            {
                item.Value.DiposePlayer();
                item.Value.Dispose();
            }
            _videoControls.Clear();
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
                if (!_videoControls.ContainsKey(index))
                {
                    RenderHost.UIInvoke(() =>
                    {
                        _videoControls[index] = new VideoControl();
                        _videoControls[index].InitPlayer();
                    });
                }
                var screen = RenderHost.GetHost(index);
                screen.ShowWallpaper(_videoControls[index], this);
                _videoControls[index].Load(wallpaper.Path);
            }
            return Task.CompletedTask;
        }
    }
}
