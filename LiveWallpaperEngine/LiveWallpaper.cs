using LiveWallpaperEngine.Models;
using LiveWallpaperEngine.Renders;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
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
        public static LiveWallpaperSetting Setting { get; private set; }
        #endregion

        #region public methods
        public static Task ApplySetting(LiveWallpaperSetting setting)
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
        public static void Show(Wallpaper wallpaper, params int[] screenIndexs)
        {
            WallpaperManager.ShowWallpaper(wallpaper, screenIndexs);
            StatusManager.ShowWallpaper(wallpaper, screenIndexs);
        }
        public static void Close(params int[] screenIndex)
        {
            WallpaperManager.CloseWallpaper(screenIndex);
            StatusManager.CloseWallpaper(screenIndex);
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
