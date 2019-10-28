using LiveWallpaperEngine.Common.Models;
using System.Threading.Tasks;

namespace LiveWallpaperEngine.Common
{
    public interface ILiveWallpaperAPI
    {
        int GetVolume();

        void SetVolume(int v);

        void Dispose();

        Task ShowWallpaper(WallpaperModel wallpaper, params int[] screenIndexs);

        void CloseWallpaper(params int[] screenIndexs);

        Task SetOptions(LiveWallpaperOptions setting);
    }
}
