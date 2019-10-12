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
        NotSupport,
        Video,
        Image,
        Web,
        Exe
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
