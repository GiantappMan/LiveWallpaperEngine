using LiveWallpaperEngine.Common.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LiveWallpaperEngine
{
    /// <summary>
    /// 壁纸渲染器
    /// </summary>
    public interface IRender
    {
        List<WallpaperType> SupportTypes { get; }
        ///// <summary>
        ///// 获取窗口句柄
        ///// </summary>
        ///// <returns></returns>
        //Task<IntPtr> GetWindowHandle();
        /// <summary>
        /// 释放
        /// </summary>
        void Dispose();
        /// <summary>
        /// 加载壁纸
        /// </summary>
        /// <param name="path"></param>
        Task LaunchWallpaper(WallpaperModel wallpaper, int screenIndex = 0);

        void Pause();

        void Resum();

        void SetVolume(int v);

        int GetVolume();
    }
}
