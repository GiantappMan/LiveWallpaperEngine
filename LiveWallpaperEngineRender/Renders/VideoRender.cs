using Mpv.NET.Player;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LiveWallpaperEngineRender.Renders
{
    public partial class VideoRender : Form
    {
        private MpvPlayer _player;

        public VideoRender()
        {
            //UI
            BackColor = Color.Magenta;
            TransparencyKey = Color.Magenta;
            InitializeComponent();
            FormBorderStyle = FormBorderStyle.None;

            //mpv
            string appDir = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            _player = new MpvPlayer(Handle, $@"{appDir}\lib\mpv-1.dll")
            {
                Loop = true,
                Volume = 0
            };

            //callback
            FormClosing += RenderForm_FormClosing;
        }

        private void RenderForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            FormClosing -= RenderForm_FormClosing;
            _player?.Dispose();
        }

        public void Mute(bool mute)
        {
            if (_player != null)
                _player.Volume = mute ? 0 : 100;
        }

        public void Pause()
        {
            _player?.Pause();
        }

        public void Resume()
        {
            _player?.Resume();
        }
    }
}
