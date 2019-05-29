using LiveWallpaperEngine;
using Mpv.NET.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Threading;

namespace LiveWallpaperEngineRender.Renders
{
    public class VideoRender : BaseRender, IVideoRender
    {
        MpvForm control = new MpvForm();
        Screen _cacheScreen;
        string _cacheAspect;

        #region properties


        #endregion

        public void Mute(bool mute)
        {
            if (control != null && control.Player != null)
                control.Player.Volume = mute ? 0 : 100;
        }

        public void Pause()
        {
            if (!Playing)
                return;

            Playing = false;
            Paused = true;

            control?.Player?.Pause();
        }

        public void Resume()
        {
            if (Playing)
                return;

            Playing = true;
            Paused = false;

            control?.Player?.Resume();
        }

        public IntPtr ShowRender()
        {
            if (control == null)
                return IntPtr.Zero;
            control.Show();
            return control.Handle;
        }

        public void Play(string path)
        {
            CurrentPath = path;
            Playing = true;

            try
            {
                if (control != null && control.Player != null)
                {
                    control.Player.Pause();
                    control.Player.Load(path);
                    control.Player.Resume();
                }
            }
            catch (Exception)
            {
            }
        }

        public void Stop()
        {
            if (!Playing && !Paused)
                return;

            Playing = Paused = false;
            control?.Player?.Stop();
        }

        public void CloseRender()
        {
            Stop();
            control?.Player?.Dispose();
            RenderDisposed = true;
            control?.Close();
            control = null;
        }

        public void Init(Screen screen)
        {
            _cacheScreen = screen;
            control.Width = screen.Bounds.Width;
            control.Height = screen.Bounds.Height;
            control.Left = screen.Bounds.Left;
            control.Top = screen.Bounds.Top;

            //mpv
            var assembly = Assembly.GetEntryAssembly();
            //单元测试
            if (assembly != null)
            {
                string appDir = System.IO.Path.GetDirectoryName(assembly.Location);
                string dllPath = $@"{appDir}\lib\mpv-1.dll";

                if (System.Environment.Is64BitProcess)
                {
                    dllPath = $@"{appDir}\lib\mpv-1-x64.dll";
                }

                control.Player = new MpvPlayer(control.Handle, dllPath)
                {
                    Loop = true,
                    Volume = 0
                };
            }
        }

        public void SetAspect(string aspect)
        {
            try
            {
                _cacheAspect = aspect;
                if (control != null && control.Player != null)
                {
                    //var test = player.API.GetPropertyString("video-aspect");
                    if (string.IsNullOrEmpty(aspect))
                        control.Player.API.SetPropertyString("video-aspect", "-1.000000");
                    else
                    {
                        //兼容中文分号
                        aspect = aspect.Replace("：", ":");
                        control.Player.API.SetPropertyString("video-aspect", aspect);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }
        }

        public IntPtr RestartRender()
        {
            IntPtr result = IntPtr.Zero;
            LiveWallpaperEngineManager.UIDispatcher.Invoke(() =>
            {
                //CloseRender(); explore死后会卡死

                if (control != null)
                {
                    control.Close();
                    control.Player = null;
                }

                control = new MpvForm();
                Init(_cacheScreen);
                ShowRender();
                result = control.Handle;
            });

            SetAspect(_cacheAspect);

            return result;
        }

        public void SetCore(LiveWallpaperEngineCore core)
        {
        }
    }
}
