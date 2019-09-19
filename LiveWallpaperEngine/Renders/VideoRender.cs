using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiveWallpaperEngine.Models;
using MpvPlayer;

namespace LiveWallpaperEngine.Renders
{
    class VideoRender : IWallpaperRender
    {
        bool _paused;
        bool _playing;
        MpvForm _control = new MpvForm();

        public WallpaperType SupportType => WallpaperType.Video;
        public string[] SupportExtensions => new string[] { ".mp4", ".flv", ".blv", ".avi" };

        public void Close()
        {
            if (!_playing && !_paused)
                return;

            _playing = _paused = false;
            _control?.Player?.Stop();
            _control?.Player?.Dispose();
            _control?.Close();
            _control = null;
        }

        public IntPtr GetWindowHandle()
        {
            if (_control == null)
                return IntPtr.Zero;
            if (_control != null && _control.Handle != IntPtr.Zero && _control.Visible)
                return _control.Handle;

            _control.InitPlayer();
            _control.Show();
            return _control.Handle;
        }

        public void Mute(bool mute)
        {
            if (_control != null && _control.Player != null)
                _control.Player.Volume = mute ? 0 : 100;
        }

        public void Pause()
        {
            if (!_playing)
                return;

            _playing = false;
            _paused = true;

            _control?.Player?.Pause();
        }

        public void Resume()
        {
            if (_playing)
                return;

            _playing = true;
            _paused = false;
            _control?.Player?.Resume();
        }

        public void LaunchWallpaper(string path)
        {
            //if (_playing)
            //    _control?.Player?.Stop();
            _playing = true;

            try
            {
                if (_control != null && _control.Player != null)
                {
                    //_control.Player.Pause();
                    _control.Player.AutoPlay = true;
                    _control.Player.Load(path, true);
                    //_control.Player.Resume();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }
        }
    }
}
