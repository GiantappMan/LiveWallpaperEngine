using System;
using System.Collections.Generic;
using System.Text;

namespace LiveWallpaperEngine
{
    public interface IRender
    {
        void Mute(bool mute);

        void Pause();

        void Resum();
    }
}
