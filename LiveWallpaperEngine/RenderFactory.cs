using LiveWallpaperEngine.Models;
using LiveWallpaperEngine.Renders;
using System;
using System.Collections.Generic;
using System.IO;
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
        public static List<IRender> Renders = new List<IRender>();

        static RenderFactory()
        {
            Renders.Add(new RemoteRender());
        }

        public static IRender GetRender(WallpaperType wType)
        {
            var render = Renders.FirstOrDefault(r => r.SupportTypes.ContainsKey(wType));
            return render;
        }
    }
}
