using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using LiveWallpaperEngine;
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
            return new Empty();
        }
        public override async Task<Empty> CloseWallpaper(CloseWallpaperRequest request, ServerCallContext context)
        {
            return new Empty();
        }
    }
}
