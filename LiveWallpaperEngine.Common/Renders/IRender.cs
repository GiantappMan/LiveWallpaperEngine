using LiveWallpaperEngine.Common.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LiveWallpaperEngine.Common.Renders
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
        Task ShowWallpaper(WallpaperModel wallpaper, params int[] screenIndex);

        void Pause();

        void Resum();

        void SetVolume(int v);

        int GetVolume();
        void CloseWallpaper(params int[] screenIndexs);
    }
}
