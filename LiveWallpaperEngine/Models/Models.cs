using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveWallpaperEngine.Models
{
    /// <summary>
    /// 壁纸类型
    /// </summary>
    public enum WallpaperType
    {
        Video,
        Image,
        Web,
        Exe,
    }
    
    /// <summary>
    /// 表示一个壁纸
    /// </summary>
    public struct Wallpaper
    {
        public WallpaperType Type { get; set; }

        public string Path { get; set; }
    }
}
