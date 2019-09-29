using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Reflection;

namespace LiveWallpaperEngineRender.Forms
{
    public partial class VideoControl : UserControl
    {
        public VideoControl()
        {
            InitializeComponent();
        }

        private Mpv.NET.Player.MpvPlayer _player;
        private string _lastPath;

        public void LoadFile(string path)
        {
            if (string.IsNullOrEmpty(path))
                return;

            _lastPath = path;

            _player?.Pause();
            _player?.Load(path);
            _player?.Resume();
        }

        public void InitPlayer()
        {
            BeginInvoke(new Action(() =>
            {
                var assembly = Assembly.GetEntryAssembly();
                string appDir = System.IO.Path.GetDirectoryName(assembly.Location);
                string dllPath = $@"{appDir}\lib\mpv-1.dll";

                //单元测试
                _player = new Mpv.NET.Player.MpvPlayer(Handle, dllPath)
                {
                    Loop = true,
                    Volume = 0
                };
                //防止视频黑边
                _player.API.SetPropertyString("panscan", "1.0");
                _player.AutoPlay = true;
                LoadFile(_lastPath);
            }));
        }

        public void DiposePlayer()
        {
            Invoke(new Action(() =>
            {
                _player?.Dispose();
                _player = null;
            }));
        }
    }
}
