using LiveWallpaperEngineAPI.Common;
using LiveWallpaperEngineAPI.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace LiveWallpaperEngineAPI.Renders
{
    public class ExeRender : IRender
    {
        MouseEventReciver mouseEventReciver = new MouseEventReciver();

        public List<WallpaperType> SupportTypes => StaticSupportTypes;
        public static List<WallpaperType> StaticSupportTypes => new List<WallpaperType>()
        {
           WallpaperType.Exe,
        };

        public ExeRender()
        {
            // 开始接收鼠标消息
            mouseEventReciver.StartRecive();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public int GetVolume(params uint[] screenIndexs)
        {
            throw new NotImplementedException();
        }

        public void Pause(params uint[] screenIndexs)
        {
        }

        public void Resum(params uint[] screenIndexs)
        {
        }

        public void SetVolume(int v, params uint[] screenIndexs)
        {
          
        }

        public Task ShowWallpaper(WallpaperModel wallpaper, params uint[] screenIndexs)
        {
            foreach (var index in screenIndexs)
            {
                var process = Process.Start(wallpaper.Path);
                // 刚创建的进程可能无法获取到MainWindowHandle属性，所以延时两秒保证属性获取正常
                Thread.Sleep(2 * 1000);
                WallpaperHelper.GetInstance(index).SendToBackground(process.MainWindowHandle);
                // 设置要接受鼠标消息的窗口的句柄
                mouseEventReciver.HTargetWindow = process.MainWindowHandle;
            }
            return Task.CompletedTask;
        }

        public void CloseWallpaper(params uint[] screenIndexs)
        {
        }
    }
}
