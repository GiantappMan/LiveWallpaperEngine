﻿using LiveWallpaperEngine;
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
    public partial class VideoRender : Form, IRender
    {
        private MpvPlayer _player;

        #region construct

        public VideoRender()
        {
            //UI
            BackColor = Color.Magenta;
            TransparencyKey = Color.Magenta;
            InitializeComponent();
            FormBorderStyle = FormBorderStyle.None;

            //mpv
            var assembly = Assembly.GetEntryAssembly();
            //单元测试
            if (assembly != null)
            {
                string appDir = System.IO.Path.GetDirectoryName(assembly.Location);
                _player = new MpvPlayer(Handle, $@"{appDir}\lib\mpv-1.dll")
                {
                    Loop = true,
                    Volume = 0
                };
            }

            //callback
            FormClosing += RenderForm_FormClosing;
        }

        private void RenderForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            FormClosing -= RenderForm_FormClosing;
            _player?.Dispose();
        }

        #endregion

        #region properties

        public bool RenderDisposed { private set; get; }

        public bool Paused { private set; get; }

        public bool Playing { private set; get; }

        #endregion

        public void Mute(bool mute)
        {
            if (_player != null)
                _player.Volume = mute ? 0 : 100;
        }

        public void Pause()
        {
            if (!Playing)
                return;

            Playing = false;
            Paused = true;

            _player?.Pause();
        }

        public void Resume()
        {
            if (Playing)
                return;

            Playing = true;
            Paused = false;

            _player?.Resume();
        }

        public IntPtr ShowRender()
        {
            Show();
            return Handle;
        }

        public void Play(string path)
        {
            Playing = true;

            if (_player != null)
            {
                _player.Pause();
                _player.Load(path);
                _player.Resume();
            }
        }

        public void Stop()
        {
            if (!Playing && !Paused)
                return;

            Playing = Paused = false;
            _player?.Stop();
        }

        public void CloseRender()
        {
            Stop();
            _player?.Dispose();
            RenderDisposed = true;
            Close();
        }
    }
}
