using LiveWallpaperEngine.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveWallpaperEngine
{
    public struct Status
    {
        /// <summary>
        /// 正在播放的显示器索引+壁纸
        /// </summary>
        public Dictionary<int, Wallpaper?> Wallpapers;
    }

    /// <summary>
    /// 状态管理
    /// 只处理状态相关数据
    /// </summary>
    static internal class StatusManager
    {
        static internal Status Status;
        static StatusManager()
        {
            Status = new Status
            {
                Wallpapers = new Dictionary<int, Wallpaper?>()
            };
        }

        static internal void ShowWallpaper(Wallpaper wallpaper, params int[] screenIndexs)
        {
            foreach (var screen in screenIndexs)
            {
                Status.Wallpapers[screen] = wallpaper;
            }
        }

        static internal void CloseWallpaper(params int[] screenIndexs)
        {
            foreach (var screen in screenIndexs)
            {
                Status.Wallpapers[screen] = null;
            }

            Status.Wallpapers.Where(m => m.Value == null).ToList()
                .ForEach(item => Status.Wallpapers.Remove(item.Key));
        }
    }
}
