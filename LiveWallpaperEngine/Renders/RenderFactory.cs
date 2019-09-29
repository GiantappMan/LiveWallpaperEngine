using LiveWallpaperEngine.Common.Models;
using LiveWallpaperEngine.Renders;
using System;
using System.Collections.Generic;
using System.IO;
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
        public static Dictionary<Type, List<WallpaperType>> Renders = new Dictionary<Type, List<WallpaperType>>();

        static RenderFactory()
        {
            Renders.Add(typeof(RemoteRender), RemoteRender.StaticSupportTypes);
            Renders.Add(typeof(ExeRender), ExeRender.StaticSupportTypes);
        }

        public static IRender CreateRender(WallpaperType.DefinedType dType)
        {
            foreach (var render in Renders)
            {
                var exist = render.Value.FirstOrDefault(m => m.DType == dType);
                if (exist != null)
                    return Activator.CreateInstance(render.Key) as IRender;
            }
            return null;
        }

        public static WallpaperType ResoveType(string filePath)
        {
            var extension = Path.GetExtension(filePath);
            foreach (var render in Renders)
            {
                var exist = render.Value.FirstOrDefault(m => m.SupportExtensions.Contains(extension.ToLower()));
                return exist;
            }
            return new WallpaperType(WallpaperType.DefinedType.NotSupport);
        }
    }
}
