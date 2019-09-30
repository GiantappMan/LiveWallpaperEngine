using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveWallpaperEngine.Common.Models
{
    public enum WalllpaperDefinedType
    {
        Video,
        Image,
        Web,
        Exe,
        NotSupport
    }

    public class WallpaperType
    {
        public WallpaperType(WalllpaperDefinedType type, params string[] extesion)
        {
            DType = type;
            SupportExtensions = extesion?.ToList();
        }
        [JsonProperty]
        public WalllpaperDefinedType DType { get; private set; }

        [JsonProperty]
        public List<string> SupportExtensions { get; private set; }
    }
}
