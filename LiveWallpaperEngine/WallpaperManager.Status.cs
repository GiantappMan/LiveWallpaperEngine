using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveWallpaperEngine
{
    public struct Status
    {
        public Dictionary<int, Wallpaper?> Wallpapers;
    }

    /// <summary>
    /// 状态管理
    /// </summary>
    public class StatusManager
    {
        public Status Status;
        public StatusManager()
        {
            Status = new Status();
            Status.Wallpapers = new Dictionary<int, Wallpaper?>();
        }

        public void ShowWallpaper(Wallpaper wallpaper, params int[] screenIndexs)
        {
            foreach (var screen in screenIndexs)
            {
                Status.Wallpapers[screen] = wallpaper;
            }
        }

        public void CloseWallpaper(params int[] screenIndexs)
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
