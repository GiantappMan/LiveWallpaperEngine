using System.Collections.Generic;
using System.Windows.Forms;
using DZY.WinAPI;
using LiveWallpaperEngine.Common;
using LiveWallpaperEngine.Common.Models;

namespace LiveWallpaperEngine.Wallpaper
{
    /// <summary>
    /// 管理所有显示器的壁纸
    /// </summary>
    static class ScreenManagers
    {
        static Dictionary<int, ScreenManager> _screenManagers = new Dictionary<int, ScreenManager>();

        internal static void Initlize()
        {
            //dpi 相关
            User32WrapperEx.SetThreadAwarenessContext(DPI_AWARENESS_CONTEXT.DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE);
            WallpaperHelper.DoSomeMagic();
            for (int i = 0; i < Screen.AllScreens.Length; i++)
            {
                _screenManagers.Add(i, new ScreenManager(i));
            }
        }
        static internal void ShowWallpaper(WallpaperModel wallpaper, params int[] screenIndexs)
        {
            if (wallpaper.Type == null)
                //根据文件路径解析type
                wallpaper.Type = RenderFactory.ResoveType(wallpaper.Path);

            if (wallpaper.Type == null)
                return;
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
