using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveWallpaperEngine.Wallpaper.Models
{
    public class WallpaperType
    {
        public enum DefinedType
        {
            Video,
            Image,
            Web,
            Exe,
            NotSupport
        }

        public WallpaperType(DefinedType type, params string[] extesion)
        {
            DType = type;
            SupportExtensions = extesion.ToList();
        }
        public DefinedType DType { get; private set; }
        public List<string> SupportExtensions { get; private set; }
    }
}
