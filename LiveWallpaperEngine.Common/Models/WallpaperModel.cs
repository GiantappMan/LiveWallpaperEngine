using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveWallpaperEngine.Common.Models
{
    /// <summary>
    /// 表示一个壁纸
    /// </summary>
    public struct WallpaperModel
    {
        public WallpaperType Type { get; set; }

        public string Path { get; set; }
    }
}
