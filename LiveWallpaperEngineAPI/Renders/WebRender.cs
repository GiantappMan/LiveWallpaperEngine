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
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        public void Resume(params string[] screens)
        {
            throw new NotImplementedException();
        }

        public void SetVolume(int v, string screen)
        {
            throw new NotImplementedException();
        }

        public Task ShowWallpaper(WallpaperModel wallpaper, params string[] screen)
        {
            return Task.Run(() =>
            {
                var assembly = Assembly.GetEntryAssembly();
                string appDir = Path.GetDirectoryName(assembly.Location);
                _driverService = ChromeDriverService.CreateDefaultService(appDir, "chromedriver.exe");

                //hide driver service command prompt window
                _driverService.HideCommandPromptWindow = true;
                ChromeOptions options = new ChromeOptions();
                options.AddArgument("disable-infobars");
                options.AddArgument("--start-maximized");

                //hide browser if you need              
                //options.AddArgument("headless");
                //or this to hiding browser
                //options.AddArgument("--window-position=-32000,-32000");

                _driver = new ChromeDriver(_driverService, options);

                if (_driver != null)
                    _driver.Navigate().GoToUrl(wallpaper.Path);
                IJavaScriptExecutor js = (IJavaScriptExecutor)_driver;
                Guid id = Guid.NewGuid();
                //string title = (string)js.ExecuteScript("return document.title");
                string title = $"livewallpaper_render {id}";
                js.ExecuteScript($"document.title = '{title}'");

                var p = Process.GetProcessesByName("chrome").First(p => p.MainWindowTitle.Contains(title));
                var h = p.MainWindowHandle;

                //hostfrom下潜桌面
                WallpaperHelper.GetInstance(screen[0]).SendToBackground(h);
                ////壁纸parent改为hostform
                //User32Wrapper.SetParent(wallpaperHandle, hostForm);
                ////把壁纸全屏铺满 hostform
                //WallpaperHelper.FullScreen(wallpaperHandle, hostForm);
            });
        }
    }
}
