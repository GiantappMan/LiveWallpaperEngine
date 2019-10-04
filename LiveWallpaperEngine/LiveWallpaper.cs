using LiveWallpaperEngine.Common;
using LiveWallpaperEngine.Common.Models;
using LiveWallpaperEngine.Renders;
using LiveWallpaperEngine.Wallpaper;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;

namespace LiveWallpaperEngine
{
    /// <summary>
    /// 巨应动态壁纸API
    /// 官网
    /// mscoder.cn
    /// giantapp.cn
    /// </summary>
    public static class LiveWallpaper
    {
        static LiveWallpaper()
        {
            RemoteRender.Initlize();
        }
        private static void ExplorerMonitor_ExpolrerCreated(object sender, EventArgs e)
        {
            try
            {
                Process.Start(Application.ResourceAssembly.Location);
                Application.Current.Shutdown();
            }
            catch (Exception)
            {

            }
        }

        #region properties
        public static Status Status
        {
            get
            {
                return StatusManager.Status;
            }
        }
        public static LiveWallpaperOptions Setting { get; private set; }
        #endregion

        #region public methods
        public static Task ApplySetting(LiveWallpaperOptions setting)
        {
            Setting = setting;

            ExplorerMonitor.ExpolrerCreated -= ExplorerMonitor_ExpolrerCreated;
            if (setting.AutoRestartWhenExplorerCrash == true)
            {
                ExplorerMonitor.ExpolrerCreated += ExplorerMonitor_ExpolrerCreated;
                ExplorerMonitor.Start();
            }
            else
                ExplorerMonitor.Stop();

            return Task.CompletedTask;
        }
        /// <summary>
        /// 显示壁纸                
        /// </summary>
        /// <remarks>       
        public static void Show(WallpaperModel wallpaper, params int[] screenIndexs)
        {
            _ = RemoteRender.ShowWallpaper(wallpaper, screenIndexs);
            StatusManager.ShowWallpaper(wallpaper, screenIndexs);
        }
        public static void Close(params int[] screenIndexs)
        {
            RemoteRender.CloseWallpaper(screenIndexs);
            StatusManager.CloseWallpaper(screenIndexs);
        }
        #endregion

        #region private methods

        /// <summary>
        /// 暂停壁纸
        /// </summary>
        /// <param name="screendIndexs"></param>
        private static void Pause(int[] screendIndexs)
        {

        }

        /// <summary>
        /// 恢复壁纸
        /// </summary>
        /// <param name="screendIndexs"></param>
        private static void Resume(int[] screendIndexs)
        {

        }

        #endregion
    }
}
