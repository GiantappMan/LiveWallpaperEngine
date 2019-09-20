using LiveWallpaperEngine.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveWallpaperEngine
{
    public class SupportExtensions : List<string> { }

    /// <summary>
    /// 壁纸渲染器
    /// </summary>
    public interface IRender
    {
        /// <summary>
        /// 支持的类型
        /// </summary>
        Dictionary<WallpaperType, SupportExtensions> SupportTypes { get; }
        /// <summary>
        /// 获取窗口句柄
        /// </summary>
        /// <returns></returns>
        IntPtr GetWindowHandle();
        /// <summary>
        /// 释放
        /// </summary>
        void Dispose();
        /// <summary>
        /// 加载壁纸
        /// </summary>
        /// <param name="path"></param>
        void LaunchWallpaper(string path);

        void Pause();

        void Resum();

        void SetVolume(int v);

        int GetVolume();
    }
}
