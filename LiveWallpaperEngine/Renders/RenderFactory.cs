using LiveWallpaperEngine.Renders;
using LiveWallpaperEngine.Wallpaper.Models;
using System.Collections.Generic;
using System.Linq;

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
        public static List<IRender> Renders = new List<IRender>();

        static RenderFactory()
        {
            Renders.Add(new RemoteRender());
        }

        public static IRender GetRender(WallpaperType.DefinedType dType)
        {
            foreach (var render in Renders)
            {
                var exist = render.SupportTypes.FirstOrDefault(m => m.DType == dType);
                if (exist != null)
                    return render;
            }
            return null;
        }
    }
}
