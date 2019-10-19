using LiveWallpaperEngine;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace LiveWallpaperEngineRemoteWebRender
{

    public class HostInfo
    {
        public bool Initlized { get; set; }
    }

    public class WebRenderAPI
    {
        private string _baseUrl;

        public WebRenderAPI(string url)
        {
            _baseUrl = url;
        }

        public async Task<HostInfo> GetInfo()
        {
            try
            {
                HttpClient client = new HttpClient();
                var json = await client.GetStringAsync(_baseUrl + "/getInfo");
                var result = JsonConvert.DeserializeObject<HostInfo>(json);
                return result;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return null;
            }
        }

        internal async Task MuteWindow(uint[] screenIndexs)
        {
            try
            {
                string ids = GetIds(screenIndexs);
                using var client = new HttpClient();
                string url = _baseUrl + $"/muteWindow?screenIndexs={ids}";
                var json = await client.GetStringAsync(url);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }

        internal async Task CloseWallpaper(uint[] screenIndexs)
        {
            try
            {
                string ids = GetIds(screenIndexs);
                using var client = new HttpClient();
                string url = _baseUrl + $"/closeWallpaper?screenIndexs={ids}";
                var json = await client.GetStringAsync(url);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }

        public async Task<Dictionary<uint, IntPtr>> ShowWallpaper(WallpaperModel wallpaper, uint[] screenIndexs)
        {
            try
            {
                string ids = GetIds(screenIndexs);
                using var client = new HttpClient();
                string url = _baseUrl + $"/showWallpaper?path={wallpaper.Path}&screenIndexs={ids}";
                var json = await client.GetStringAsync(url);
                var result = JsonConvert.DeserializeObject<Dictionary<uint, IntPtr>>(json);
                return result;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return null;
            }
        }

        private string GetIds(uint[] ids)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var i in ids)
            {
                sb.AppendFormat("{0},", i);
            }
            return sb.ToString();
        }
    }
}
