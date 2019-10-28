using LiveWallpaperEngine.Common.Models;
using LiveWallpaperEngine.Common.Renders;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LiveWallpaperEngineRender.Renders
{
    abstract class BaseRender<RenderControl> : IRender where RenderControl : Control, IRenderControl, new()
    {
        //屏幕索引和对应的控件
        protected readonly Dictionary<int, RenderControl> _controls = new Dictionary<int, RenderControl>();

        public virtual List<WallpaperType> SupportTypes => throw new NotImplementedException();

        public virtual void CloseWallpaper(params int[] screenIndexs)
        {
            foreach (var (screenIndex, control) in GetControls(screenIndexs))
            {
                control.Stop();
                var screen = RenderHost.GetHost(screenIndex);
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

        public int GetVolume(params int[] screenIndexs)
        {
            throw new NotImplementedException();
        }


        public virtual void Pause(params int[] screenIndexs)
        {
            foreach (var (screenIndex, control) in GetControls(screenIndexs))
                control.Pause();
        }

        public virtual void Resum(params int[] screenIndexs)
        {
            foreach (var (screenIndex, control) in GetControls(screenIndexs))
                control.Resum();
        }

        public void SetVolume(int v, params int[] screenIndexs)
        {
            foreach (var (screenIndex, control) in GetControls(screenIndexs))
                control.SetVolume(v);
        }



        public virtual Task ShowWallpaper(WallpaperModel wallpaper, params int[] screenIndexs)
        {
            foreach (var index in screenIndexs)
            {
                RenderHost.UIInvoke(() =>
                {
                    if (!_controls.ContainsKey(index))
                    {
                        _controls[index] = new RenderControl();
                        _controls[index].InitRender();
                    }
                    var screen = RenderHost.GetHost(index);
                    screen.ShowWallpaper(_controls[index]);
                    _controls[index].Load(wallpaper.Path);
                });
            }
            return Task.CompletedTask;
        }

        protected IEnumerable<(int screenIndex, RenderControl control)> GetControls(params int[] screenIndexs)
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
