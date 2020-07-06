using Giantapp.LiveWallpaper.Engine.Renders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Giantapp.LiveWallpaper.Engine
{
    public static class WallpaperManager
    {
        private static Dispatcher _uiDispatcher;
        public static string[] Screens { get; private set; }
        public static LiveWallpaperOptions Options { get; private set; }

        //Dictionary<DeviceName，WallpaperModel>
        public static Dictionary<string, WallpaperModel> CurrentWalpapers { get; private set; } = new Dictionary<string, WallpaperModel>();

        public static void Initlize(Dispatcher dispatcher)
        {
            RenderFactory.Renders.Add(new ExeRender());
            RenderFactory.Renders.Add(new VideoRender());
            _uiDispatcher = dispatcher;
        }

        internal static void UIInvoke(Action a)
        {
            _uiDispatcher.Invoke(a);
        }
        public static Task<object> GetWallpapers(string dir)
        {
            throw new NotImplementedException();
        }

        public static async Task<bool> DeleteWallpaperPack(string absolutePath)
        {
            throw new NotImplementedException();
        }

        public static async Task<bool> UpdateWallpaper(object source, object newWP)
        {
            throw new NotImplementedException();
        }

        public static async Task<bool> CreateWallpaper(object newWP)
        {
            throw new NotImplementedException();
        }

        public static async Task ShowWallpaper(WallpaperModel wallpaper, params string[] screens)
        {
            IRender currentRender;
            if (wallpaper.Type == null)
                currentRender = RenderFactory.GetRender(Path.GetExtension(wallpaper.Path));
            else
                currentRender = RenderFactory.GetRender(wallpaper.Type.Value);

            if (currentRender == null)
                if (wallpaper.Type == null)
                    throw new ArgumentException("Unsupported wallpaper type");

            //关闭之前的壁纸
            CloseWallpaper(screens);

            await currentRender.ShowWallpaper(wallpaper, screens);

            foreach (var index in screens)
            {
                if (!CurrentWalpapers.ContainsKey(index))
                    CurrentWalpapers.Add(index, wallpaper);
                else
                    CurrentWalpapers[index] = wallpaper;
            }

            //ApplyAudioSource();
        }


        public static void CloseWallpaper(params string[] screens)
        {
            foreach (var screenItem in screens)
            {
                if (CurrentWalpapers.ContainsKey(screenItem))
                    CurrentWalpapers.Remove(screenItem);
            }

            RenderFactory.Renders.ForEach(m => m.CloseWallpaper(screens));
        }

        public static Task SetOptions(LiveWallpaperOptions setting)
        {
            Options = setting;
            return Task.CompletedTask;
        }
    }
}
