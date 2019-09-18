using LiveWallpaperEngine.Models;
using LiveWallpaperEngine.Renders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveWallpaperEngine
{
    /// <summary>
    /// 定义render类型的实现
    /// </summary>
    /// <typeparam name="Render"></typeparam>
    public class RenderFactory
    {
        /// <summary>
        /// 公开属性，方便外部库自定义
        /// </summary>
        public static Dictionary<WallpaperType, Type> RenderMaps = new Dictionary<WallpaperType, Type>();

        static RenderFactory()
        {
            RenderMaps.Add(WallpaperType.Exe, typeof(ExeRender));
            RenderMaps.Add(WallpaperType.Image, typeof(ImageRender));
            RenderMaps.Add(WallpaperType.Video, typeof(VideoRender));
            RenderMaps.Add(WallpaperType.Web, typeof(WebRender));
        }

        public static IWallpaperRender GetRender(WallpaperType wType)
        {
            var render = Activator.CreateInstance(RenderMaps[wType]) as IWallpaperRender;
            return render;
        }
    }
}
