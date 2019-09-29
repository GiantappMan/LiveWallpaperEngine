using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveWallpaperEngine.Common.Models
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
            SupportExtensions = extesion?.ToList();
        }
        [JsonProperty]
        public DefinedType DType { get; private set; }

        [JsonProperty]
        public List<string> SupportExtensions { get; private set; }
    }
}
