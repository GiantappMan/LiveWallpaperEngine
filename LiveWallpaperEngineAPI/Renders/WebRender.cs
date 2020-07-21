using DZY.WinAPI;
using Giantapp.LiveWallpaper.Engine.Utils;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Giantapp.LiveWallpaper.Engine.Renders
{
    public class WebRender : IRender
    {
        #region fields
        IWebDriver _driver;
        ChromeDriverService _driverService;
        #endregion

        public WallpaperType SupportedType => WallpaperType.Web;

        public List<string> SupportedExtension => new List<string>() { ".html", ".htm" };

        public void CloseWallpaper(params string[] screens)
        {
        }

        public void Dispose()
        {
            _driver.Close();
            _driver.Quit();
            _driver.Dispose();
            _driverService.Dispose();
        }

        public int GetVolume(string screen)
        {
            throw new NotImplementedException();
        }

        public void Pause(params string[] screens)
        {

        }

        public void Resume(params string[] screens)
        {

        }

        public void SetVolume(int v, string screen)
        {
        }

        public Task ShowWallpaper(WallpaperModel wallpaper, params string[] screen)
        {
            return Task.Run(() =>
            {
                var assembly = Assembly.GetEntryAssembly();
                string appDir = Path.GetDirectoryName(assembly.Location);
                if (_driverService == null)
                {
                    string driverName = "chromedriver";
                    var oldPs = Process.GetProcessesByName(driverName);
                    foreach (var pItem in oldPs)
                        pItem.Kill();

                    _driverService = ChromeDriverService.CreateDefaultService(appDir, $"{driverName}.exe");
                    //hide driver service command prompt window
                    _driverService.HideCommandPromptWindow = true;
                }
                ChromeOptions options = new ChromeOptions();
                List<string> ls = new List<string>();
                // 禁止自动化提示
                ls.Add("enable-automation");
                options.AddExcludedArguments(ls);
                // 禁用自动化扩展
                options.AddAdditionalCapability("useAutomationExtension", false);
                //// 修改初始位置
                //options.AddArgument("--window-position=-32000,-32000");
                // 全屏
                options.AddArgument($"--start-fullscreen {wallpaper.Path}");
                //// kiosk 模式 ，无边框。好像不能多开
                //options.AddArgument($"--kiosk {wallpaper.Path}");

                _driver = new ChromeDriver(_driverService, options);

                if (_driver != null)
                    _driver.Navigate().GoToUrl(wallpaper.Path);
                IJavaScriptExecutor js = (IJavaScriptExecutor)_driver;
                Guid id = Guid.NewGuid();
                //string title = (string)js.ExecuteScript("return document.title");
                string title = $"livewallpaper_render {id}";
                js.ExecuteScript($"document.title = '{title}'");
                //js.ExecuteScript("document.body.webkitRequestFullscreen()");

                Process p = null;
                while (p == null)
                {
                    p = Process.GetProcessesByName("chrome").FirstOrDefault(p => p.MainWindowTitle.Contains(title));
                    Thread.Sleep(10);
                }
                var h = p.MainWindowHandle;

                //hostfrom下潜桌面
                WallpaperHelper.GetInstance(screen[0]).SendToBackground(h);
                ////壁纸parent改为hostform
                //User32Wrapper.SetParent(wallpaperHandle, hostForm);
                ////把壁纸全屏铺满 hostform
                //WallpaperHelper.FullScreen(wallpaperHandle, hostForm);

                ////消除游戏边框
                //var style = User32Wrapper.GetWindowLong(h, WindowLongFlags.GWL_STYLE);
                //style &= ~(int)(WindowStyles.WS_EX_TOOLWINDOW | WindowStyles.WS_CAPTION | WindowStyles.WS_THICKFRAME | WindowStyles.WS_MINIMIZEBOX | WindowStyles.WS_MAXIMIZEBOX | WindowStyles.WS_SYSMENU);
                //User32Wrapper.SetWindowLong(h, WindowLongFlags.GWL_STYLE, style);
            });
        }
    }
}
