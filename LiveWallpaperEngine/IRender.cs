using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace LiveWallpaperEngine
{
    public interface IRender
    {
        string CurrentPath { get; }
        bool RenderDisposed { get; }
        bool Paused { get; }
        bool Playing { get; }

        void Mute(bool mute);

        void Play(string path);

        void Stop();

        void Pause();

        void Resume();

        void InitRender(Screen screen);

        IntPtr ShowRender();

        void CloseRender();

        IntPtr RestartRender();
    }
}
