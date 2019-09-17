using LiveWallpaperEngine.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveWallpaperEngine
{
    /// <summary>
    /// 壁纸渲染器
    /// </summary>
    public interface IWallpaperRender
    {
        /// <summary>
        /// 支持的类型
        /// </summary>
        WallpaperType SupportType { get; }

        /// <summary>
        /// 支持的文件名后缀 例如：.html
        /// </summary>
        string[] SupportExtensions { get; }
    }
}
