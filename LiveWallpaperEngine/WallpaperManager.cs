using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DZY.WinAPI;
using LiveWallpaperEngine.Models;

namespace LiveWallpaperEngine
{
    /// <summary>
    /// 管理所有显示器的壁纸
    /// </summary>
    static class WallpaperManager
    {
        static Dictionary<int, WallpaperScreenManager> _screenManagers = new Dictionary<int, WallpaperScreenManager>();

        static WallpaperManager()
        {
            Initlize();
        }
        private static void Initlize()
        {
            //dpi 相关
            User32WrapperEx.SetThreadAwarenessContext(DPI_AWARENESS_CONTEXT.DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE);
            for (int i = 0; i < Screen.AllScreens.Length; i++)
            {
                var item = Screen.AllScreens[i];
                _screenManagers.Add(i, new WallpaperScreenManager(item));
            }
        }
        static internal void ShowWallpaper(Wallpaper wallpaper, params int[] screenIndexs)
        {
            foreach (var index in screenIndexs)
            {                
                _screenManagers[index].ShowWallpaper(wallpaper);
            }
        }
        static internal void CloseWallpaper(params int[] screenIndexs)
        {
            foreach (var index in screenIndexs)
            {
                _screenManagers[index].Close();
            }
        }
    }
}
