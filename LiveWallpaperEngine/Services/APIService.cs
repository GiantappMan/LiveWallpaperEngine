using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
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
            //System.Windows.Forms.Form f = new System.Windows.Forms.Form();
            //f.Show();
            await Task.Delay(100);
            return new Empty();
        }
        public override Task<Empty> CloseWallpaper(CloseWallpaperRequest request, ServerCallContext context)
        {
            return Task.FromResult(new Empty());
        }

        public override Task<Empty> SetOptions(LiveWallpaperOptions request, ServerCallContext context)
        {
            return Task.FromResult(new Empty());
        }
    }
}
