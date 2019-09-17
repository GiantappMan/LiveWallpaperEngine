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
        //不支持的壁纸
        NotSupport
    }

    /// <summary>
    /// 表示一个壁纸
    /// </summary>
    public struct Wallpaper
    {
        /// <summary>
        /// 如果没有指定，内部根件后缀判断
        /// </summary>
        public WallpaperType? Type { get; set; }

        public string Path { get; set; }
    }
}
