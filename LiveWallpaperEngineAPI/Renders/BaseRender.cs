using LiveWallpaperEngineAPI.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LiveWallpaperEngineAPI.Renders
{
    abstract class BaseRender<RenderControl> : IRender where RenderControl : Control, IRenderControl, new()
    {
        //屏幕索引和对应的控件
        protected readonly Dictionary<uint, RenderControl> _controls = new Dictionary<uint, RenderControl>();

        public virtual List<WallpaperType> SupportTypes => throw new NotImplementedException();

        public virtual void CloseWallpaper(params uint[] screenIndexs)
        {
            foreach (var (screenIndex, control) in GetControls(screenIndexs))
            {
                control.Stop();
                var screen = RenderHost.GetHost(screenIndex, false);
                if (screen != null)
                    screen.RemoveWallpaper(control);
            }
        }

        public virtual void Dispose()
        {
            foreach (var item in _controls)
            {
                item.Value.DisposeRender();
                item.Value.Dispose();
            }
            _controls.Clear();
        }

        public int GetVolume(params uint[] screenIndexs)
        {
            throw new NotImplementedException();
        }

        public virtual void Pause(params uint[] screenIndexs)
        {
            foreach (var (screenIndex, control) in GetControls(screenIndexs))
                control.Pause();
        }

        public virtual void Resum(params uint[] screenIndexs)
        {
            foreach (var (screenIndex, control) in GetControls(screenIndexs))
                control.Resum();
        }

        public void SetVolume(int v, params uint[] screenIndexs)
        {
            foreach (var (screenIndex, control) in GetControls(screenIndexs))
                control.SetVolume(v);
        }


        public virtual Task ShowWallpaper(WallpaperModel wallpaper, params uint[] screenIndexs)
        {
            foreach (var index in screenIndexs)
            {
                if (!_controls.ContainsKey(index))
                {
                    _controls[index] = new RenderControl();
                    _controls[index].InitRender();
                }
                var screen = RenderHost.GetHost(index);
                screen!.ShowWallpaper(_controls[index]);
                _controls[index].Load(wallpaper.Path);
            }
            return Task.CompletedTask;
        }

        protected IEnumerable<(uint screenIndex, RenderControl control)> GetControls(params uint[] screenIndexs)
        {
            foreach (var index in screenIndexs)
            {
                if (!_controls.ContainsKey(index))
                    continue;

                yield return (index, _controls[index]);
            }
        }
    }
}
