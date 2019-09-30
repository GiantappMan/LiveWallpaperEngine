using DZY.WinAPI;
using LiveWallpaperEngine.Common;
using LiveWallpaperEngine.Common.Models;
using LiveWallpaperEngine.Common.Renders;
using LiveWallpaperEngine.Renders;

namespace LiveWallpaperEngine.Wallpaper
{
    /// <summary>
    /// 管理所有显示器的壁纸
    /// </summary>
    static class ScreenManagers
    {
        internal static void Initlize()
        {
            //注册render
            RenderFactory.Renders.Add(typeof(RemoteRender), RemoteRender.StaticSupportTypes);
            RenderFactory.Renders.Add(typeof(ExeRender), ExeRender.StaticSupportTypes);

            //dpi 相关
            User32WrapperEx.SetThreadAwarenessContext(DPI_AWARENESS_CONTEXT.DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE);
            WallpaperHelper.DoSomeMagic();
        }

        static internal void ShowWallpaper(WallpaperModel wallpaper, params int[] screenIndexs)
        {
            if (wallpaper.Type == null)
                //根据文件路径解析type
                wallpaper.Type = RenderFactory.ResoveType(wallpaper.Path);

            if (wallpaper.Type == null)
                return;

            var _currentRender = RenderFactory.GetOrCreateRender(wallpaper.Type.DType);
            _currentRender.ShowWallpaper(wallpaper, screenIndexs);
        }

        static internal void CloseWallpaper(params int[] screenIndexs)
        {
            foreach (var render in RenderFactory.CacheInstance)
            {
                render.CloseWallpaper(screenIndexs);
            }
        }
    }
}
