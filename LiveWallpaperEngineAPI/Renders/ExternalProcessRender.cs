using DZY.WinAPI;
using DZY.WinAPI.Desktop.API;
using DZY.WinAPI.Extension;
using DZY.WinAPI.Helpers;
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
    //目前所有动态壁纸都是这个类实现，通过启用外部exe来渲染，以防止崩溃。
    public class ExternalProcessRender : BaseRender
    {
        private static ProcessJobTracker _pj = new ProcessJobTracker();
        //private List<RenderInfo> _playingRenders = new List<RenderInfo>();

        protected ExternalProcessRender(WallpaperType type, List<string> extension, bool mouseEvent = true) : base(type, extension, mouseEvent)
        {
        }

        protected override async Task InnerCloseWallpaperAsync(List<RenderInfo> wallpaperRenders, bool isTemporary)
        {
            //不论是否临时关闭，都需要关闭进程重启进程

            foreach (var render in wallpaperRenders)
            {
                try
                {
                    var p = Process.GetProcessById(render.PId);
                    p.Kill();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"InnerCloseWallpaper ex:{ex}");
                }
                finally
                {
                    if (SupportMouseEvent)
                        await DesktopMouseEventReciver.RemoveHandle(render.ReceiveMouseEventHandle);
                }
            }
        }

        protected override async Task<List<RenderInfo>> InnerShowWallpaper(WallpaperModel wallpaper, CancellationToken ct, params string[] screens)
        {
            //var changedRender = _playingRenders.Where(m => screens.Contains(m.Screen) || wallpaper.Path != m.Wallpaper.Path).ToList();
            ////关闭 已经修改壁纸路径的render，重启开进程            
            //if (changedRender.Count > 0)
            //    await InnerCloseWallpaper(changedRender);

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
                        if (processResult.HostHandle == IntPtr.Zero)
                            return;

                        var host = LiveWallpaperRenderForm.GetHost(screenItem);
                        host!.ShowWallpaper(processResult.HostHandle);

                        result.Add(new RenderInfo(processResult)
                        {
                            Wallpaper = wallpaper,
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

            if (SupportMouseEvent && WallpaperManager.Options.ForwardMouseEvent && wallpaper.EnableMouseEvent)
            {
                foreach (var item in result)
                    await DesktopMouseEventReciver.AddHandle(item.ReceiveMouseEventHandle, item.Screen);
            }
            return result;
        }

        protected virtual ProcessStartInfo GenerateProcessInfo(string path)
        {
            return new ProcessStartInfo(path);
        }
        protected virtual Task<RenderProcess> StartProcess(string path, CancellationToken ct)
        {
            return Task.Run(() =>
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                int timeout = 30 * 1000;

                ProcessStartInfo info = GenerateProcessInfo(path);
                if (info == null)
                    return new RenderProcess();
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

                RenderProcess result = new RenderProcess()
                {
                    PId = targetProcess.Id,
                    HostHandle = targetProcess.MainWindowHandle,
                    ReceiveMouseEventHandle = targetProcess.MainWindowHandle
                };
                //壁纸引擎关闭后，关闭渲染进程
                _pj.AddProcess(targetProcess);
                targetProcess.Dispose();
                return result;
            });
        }
    }
}
