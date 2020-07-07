using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using LiveWallpaperEngineAPI;
using Microsoft.Extensions.Logging;

namespace LiveWallpaperEngine
{
    public class APIService : API.APIBase
    {
        private readonly ILogger<APIService> _logger;
        public APIService(ILogger<APIService> logger)
        {
            _logger = logger;
        }

        public override async Task<Empty> ShowWallpaper(ShowWallpaperRequest request, ServerCallContext context)
        {
            await WallpaperManager.Instance.ShowWallpaper(request.Wallpaper, request.ScreenIndexs.ToArray());
            return new Empty();
        }

        public override Task<Empty> CloseWallpaper(CloseWallpaperRequest request, ServerCallContext context)
        {
            WallpaperManager.Instance.CloseWallpaper(request.ScreenIndexs.ToArray());
            return Task.FromResult(new Empty());
        }

        public override async Task<Empty> SetOptions(LiveWallpaperOptions request, ServerCallContext context)
        {
            await WallpaperManager.Instance.SetOptions(request);
            return new Empty();
        }
    }
}
