using Giantapp.LiveWallpaper.Engine.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Giantapp.LiveWallpaper.Engine.Renders
{
    /// <summary>
    /// 壁纸渲染器
    /// </summary>
    public interface IRender
    {
        List<WallpaperType> SupportTypes { get; }
        /// <summary>
        /// 释放
        /// </summary>
        void Dispose();
        /// <summary>
        /// 加载壁纸
        /// </summary>
        /// <param name="path"></param>
        Task ShowWallpaper(WallpaperModel wallpaper, params uint[] screenIndex);

        void Pause(params uint[] screenIndexs);

        void Resum(params uint[] screenIndexs);

        void SetVolume(int v, params uint[] screenIndexs);

        int GetVolume(params uint[] screenIndexs);
        void CloseWallpaper(params uint[] screenIndexs);
    }
}
