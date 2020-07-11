using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Giantapp.LiveWallpaper.Engine
{
    public interface IRender
    {
        WallpaperType SupportedType { get; }

        List<string> SupportedExtension { get; }
        /// <summary>
        /// 释放
        /// </summary>
        void Dispose();
        /// <summary>
        /// 加载壁纸
        /// </summary>
        /// <param name="path"></param>
        Task ShowWallpaper(WallpaperModel wallpaper, params string[] screen);
        void Pause(params string[] screens);
        void Resume(params string[] screens);
        void SetVolume(int v, string screen);
        int GetVolume(string screen);
        void CloseWallpaper(params string[] screens);
    }
}
