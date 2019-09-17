using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiveWallpaperEngine.Models;

namespace LiveWallpaperEngine
{
    /// <summary>
    /// 管理所有显示器的壁纸
    /// </summary>
    static class WallpaperManager
    {
        static List<WallpaperScreenManager> _screenManagers = new List<WallpaperScreenManager>();

        static internal void ShowWallpaper(string path, IWallpaperRender render, params int[] screenIndexs)
        {
        }
        static internal void CloseWallpaper(params int[] screenIndex)
        {
        }
    }
}
