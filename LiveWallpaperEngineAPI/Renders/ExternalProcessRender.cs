using DZY.WinAPI;
using DZY.WinAPI.Desktop.API;
using DZY.WinAPI.Extension;
using Giantapp.LiveWallpaper.Engine.Forms;
using Giantapp.LiveWallpaper.Engine.Utils;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Forms;

namespace Giantapp.LiveWallpaper.Engine.Renders
{
    //目前所有壁纸都是这个类实现，通过启用外部exe来渲染，以防止崩溃。
    public class ExternalProcessRender : BaseRender
    {
        protected ExternalProcessRender(WallpaperType type, List<string> extension) : base(type, extension)
        {
        }

        protected override Task InnerCloseWallpaper(List<RenderInfo> playingWallpaper)
        {
            return Task.Run(() =>
            {
                foreach (var wallpapaer in playingWallpaper)
                {
                    try
                    {
                        var p = Process.GetProcessById(wallpapaer.PId);
                        p.Kill();
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"InnerCloseWallpaper ex:{ex}");
                    }
                }
            });
        }

        protected override async Task<List<RenderInfo>> InnerShowWallpaper(WallpaperModel wallpaper, CancellationToken ct, params string[] screens)
        {
            List<RenderInfo> result = new List<RenderInfo>();
            List<Task> tmpTasks = new List<Task>();
            foreach (var screenItem in screens)
            {
                if (ct.IsCancellationRequested)
                    break;
                var task = Task.Run(async () =>
                {
                    try
                    {
                        var processResult = await StartProcess(wallpaper.Path, ct);

                        //壁纸启动失败
                        if (processResult.Handle == IntPtr.Zero)
                            return;

                        var host = LiveWallpaperRenderForm.GetHost(screenItem);
                        host!.ShowWallpaper(processResult.Handle);

                        result.Add(new RenderInfo()
                        {
                            Wallpaper = wallpaper,
                            ReceiveMouseEventHandle = processResult.Handle,
                            PId = processResult.PId,
                            Screen = screenItem
                        });
                    }
                    catch (OperationCanceledException)
                    {
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex);
                    }
                    finally
                    {
                    }
                }, ct);

                tmpTasks.Add(task);
            }
            await Task.WhenAll(tmpTasks);
            return result;
        }

        protected virtual Task<(IntPtr Handle, int PId)> StartProcess(string path, CancellationToken ct)
        {
            return Task.Run(() =>
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                int timeout = 30 * 1000;

                ProcessStartInfo info = GenerateProcessInfo(path);
                if (info == null)
                    return (IntPtr.Zero, -1);
                info.WindowStyle = ProcessWindowStyle.Maximized;
                info.CreateNoWindow = true;
                Process targetProcess = Process.Start(info);

                while (targetProcess.MainWindowHandle == IntPtr.Zero)
                {
                    if (ct.IsCancellationRequested)
                        targetProcess.Kill();
                    ct.ThrowIfCancellationRequested();
                    Thread.Sleep(10);
                    int pid = targetProcess.Id;
                    targetProcess.Dispose();
                    //mainWindowHandle不会变，重新获取
                    targetProcess = Process.GetProcessById(pid);

                    if (sw.ElapsedMilliseconds > timeout)
                    {
                        sw.Stop();
                        break;
                    }
                }

                (IntPtr Handle, int PId) result = (targetProcess.MainWindowHandle, targetProcess.Id);
                targetProcess.Dispose();
                return result;
            });
        }
    }
}
