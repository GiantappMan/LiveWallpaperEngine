using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading.Tasks;
using LiveWallpaperEngine;
using LiveWallpaperEngineAPI.Common;
using LiveWallpaperEngineAPI.Forms;

namespace LiveWallpaperEngineAPI.Renders
{
    class WebRender : IRender
    {
        Process _renderProcess = null;
        public static List<WallpaperType> StaticSupportTypes => new List<WallpaperType>()
        {
           ConstWallpaperTypes.DefinedType[WalllpaperDefinedType.Web],
        };
        public List<WallpaperType> SupportTypes => StaticSupportTypes;

        public void CloseWallpaper(params int[] screenIndexs)
        {
            throw new System.NotImplementedException();
        }

        public void Dispose()
        {
            throw new System.NotImplementedException();
        }

        public int GetVolume(params int[] screenIndexs)
        {
            throw new System.NotImplementedException();
        }

        public void Pause(params int[] screenIndexs)
        {
            throw new System.NotImplementedException();
        }

        public void Resum(params int[] screenIndexs)
        {
            throw new System.NotImplementedException();
        }

        public void SetVolume(int v, params int[] screenIndexs)
        {
            throw new System.NotImplementedException();
        }

        public async Task ShowWallpaper(WallpaperModel wallpaper, params int[] screenIndexs)
        {
            if (_renderProcess == null || _renderProcess.HasExited)
            {
                var renderAPIPort = GetAvailablePort(9000);
                _renderProcess = Process.Start("", renderAPIPort.ToString());
            }

            foreach (var scIndex in screenIndexs)
            {
                IntPtr windowHandle = await GetHostFromRemote(scIndex);
                WallpaperHelper.GetInstance(scIndex).SendToBackground(windowHandle);
            }
            throw new System.NotImplementedException();
        }
        public static int GetAvailablePort(int startingPort)
        {
            var properties = IPGlobalProperties.GetIPGlobalProperties();

            //getting active connections
            var tcpConnectionPorts = properties.GetActiveTcpConnections()
                                .Where(n => n.LocalEndPoint.Port >= startingPort)
                                .Select(n => n.LocalEndPoint.Port);

            //getting active tcp listners - WCF service listening in tcp
            var tcpListenerPorts = properties.GetActiveTcpListeners()
                                .Where(n => n.Port >= startingPort)
                                .Select(n => n.Port);

            //getting active udp listeners
            var udpListenerPorts = properties.GetActiveUdpListeners()
                                .Where(n => n.Port >= startingPort)
                                .Select(n => n.Port);

            var port = Enumerable.Range(startingPort, ushort.MaxValue)
                .Where(i => !tcpConnectionPorts.Contains(i))
                .Where(i => !tcpListenerPorts.Contains(i))
                .Where(i => !udpListenerPorts.Contains(i))
                .FirstOrDefault();

            return port;
        }

        private Task<IntPtr> GetHostFromRemote(int scIndex)
        {
            throw new NotImplementedException();
        }
    }
}
