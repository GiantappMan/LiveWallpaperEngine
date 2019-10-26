using LiveWallpaperEngineAPI;
using LiveWallpaperEngineAPI.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace LiveWallpaperEngineAPI.Renders
{
    /// <summary>
    /// 定义render类型的实现
    /// </summary>
    /// <typeparam name="Render"></typeparam>
    public static class RenderFactory
    {
        /// <summary>
        /// 公开属性，方便外部库自定义
        /// </summary>
        public static Dictionary<Type, List<WallpaperType>> Renders = new Dictionary<Type, List<WallpaperType>>();
        public static List<IRender> CacheInstance = new List<IRender>();


        public static IRender CreateRender(WallpaperType dType)
        {
            foreach (var render in Renders)
            {
                var exist = render.Value.FirstOrDefault(m => m == dType);
                if (exist != WallpaperType.NotSupport)
                    return Activator.CreateInstance(render.Key) as IRender;
            }
            return null;
        }

        public static IRender GetOrCreateRender(WallpaperType dType)
        {
            foreach (var instance in CacheInstance)
            {
                var exist = instance.SupportTypes.FirstOrDefault(m => m == dType);
                if (exist != WallpaperType.NotSupport)
                    return instance;
            }

            var result = CreateRender(dType);
            if (result != null)
                CacheInstance.Add(result);

            return result;
        }

        public static WallpaperType ResoveType(string filePath)
        {
            var extension = Path.GetExtension(filePath);
            foreach (var render in Renders)
            {
                var exist = render.Value.FirstOrDefault(m => ConstWallpaperTypes.DefinedType[m].Contains(extension.ToLower()));
                if (exist != WallpaperType.NotSupport)
                    return exist;
            }
            return WallpaperType.NotSupport;
        }
    }
}
