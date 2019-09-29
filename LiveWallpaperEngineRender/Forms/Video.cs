using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace LiveWallpaperEngineRender.Forms
{
    public partial class Video : Form
    {
        private Mpv.NET.Player.MpvPlayer _player;
        public Video()
        {
            //UI
            BackColor = Color.Magenta;
            TransparencyKey = Color.Magenta;
            ShowInTaskbar = false;
            InitializeComponent();
            FormBorderStyle = FormBorderStyle.None;

            //callback
            FormClosing += RenderForm_FormClosing;
            InitPlayer();
        }

        public void LoadFile(string path)
        {
            _player.Pause();
            _player.Load(path);
            _player.Resume();
        }

        private void InitPlayer()
        {
            Invoke(new Action(() =>
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

        private void RenderForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            FormClosing -= RenderForm_FormClosing;
            _player?.Dispose();
            _player = null;
        }
    }
}
