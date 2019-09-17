using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveWallpaperEngine
{
    /// <summary>
    /// 壁纸管理
    /// </summary>
    public static class WallpaperManager
    {
        private static StatusManager _statusManager = new StatusManager();

        public static Status Status
        {
            get
            {
                return _statusManager.Status;
            }
        }

        public static WallpaperManagerSetting GlobalSetting { get; private set; }

        public static Task ApplySetting(WallpaperManagerSetting setting)
        {
            return Task.CompletedTask;
        }

        #region public methods

        public static WallpaperType GetWallpaperType(string file)
        {
            return WallpaperType.Automatic;
        }

        /// <summary>
        /// 显示壁纸                
        /// </summary>
        /// <remarks>       
        public static void ShowWallpaper(Wallpaper wallpaper, params int[] screenIndexs)
        {
            _statusManager.ShowWallpaper(wallpaper, screenIndexs);
        }

        public static void CloseWallpaper(params int[] screenIndex)
        {
            _statusManager.CloseWallpaper(screenIndex);
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
