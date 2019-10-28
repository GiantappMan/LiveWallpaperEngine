using System;
using System.Collections.Generic;
using System.Text;

namespace LiveWallpaperEngine
{
    public interface IVideoRender : IRender
    {
        string CurrentPath { get; }

        bool Paused { get; }

        bool Playing { get; }

        void Mute(bool mute);

        void Play(string path);

        void Stop();

        void Pause();

        void Resume();
    }
}
