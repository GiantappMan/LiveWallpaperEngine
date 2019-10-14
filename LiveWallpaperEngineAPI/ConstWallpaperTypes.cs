using LiveWallpaperEngine;
using System.Collections.Generic;

namespace LiveWallpaperEngineAPI
{
    public static class ConstWallpaperTypes
    {
        public static Dictionary<WalllpaperDefinedType, WallpaperType> DefinedType = new Dictionary<WalllpaperDefinedType, WallpaperType>()
        {
            { WalllpaperDefinedType.Exe,new WallpaperType(){DType=WalllpaperDefinedType.Exe}  },
            { WalllpaperDefinedType.Video,new WallpaperType(){DType=WalllpaperDefinedType.Video}  },
            { WalllpaperDefinedType.Image,new WallpaperType(){DType=WalllpaperDefinedType.Image}  },
            { WalllpaperDefinedType.Web,new WallpaperType(){DType=WalllpaperDefinedType.Web}  },
        };
        static ConstWallpaperTypes()
        {
            DefinedType[WalllpaperDefinedType.Exe].SupportExtensions.AddRange(new List<string> { ".exe" });
            DefinedType[WalllpaperDefinedType.Video].SupportExtensions.AddRange(new List<string> { ".mp4", ".flv", ".blv", ".avi" });
            DefinedType[WalllpaperDefinedType.Image].SupportExtensions.AddRange(new List<string> { ".mp4", ".flv", ".blv", ".avi" });
            DefinedType[WalllpaperDefinedType.Web].SupportExtensions.AddRange(new List<string> { ".html", ".htm" });
        }
    }
}
