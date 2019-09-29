using LiveWallpaperEngine.Wallpaper.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Text;
using System.Threading.Tasks;
using TinyIpc.Messaging;

namespace LiveWallpaperEngine.Common
{
    #region ToServerCommands
    public struct LaunchWallpaperResult
    {
        public IntPtr Handle { get; set; }
    }

    public struct Ready { }

    #endregion

    #region ToClientCommands

    public class LaunchWallpaper
    {
        public WallpaperModel Wallpaper { get; set; }
        public int ScreenIndex { get; set; }
    }

    #endregion

    public class Command
    {
        // 发送方id
        public string FromId { get; set; }
        // 接收人id，如果为空。都可以接收
        public string TargetID { get; set; }
        // 命令标识
        public string CommandFullName { get; set; }
        public string Parameter { get; set; }
    }

    class IPCHelper : IDisposable
    {
        public const string ServerID = "LivewallpaperServerIPC";
        public const string RemoteRenderID = "LivewallpaperRemoteRenderIPC";

        TinyMessageBus _messageBus;
        ConcurrentQueue<Command> _messages = new ConcurrentQueue<Command>();
        public event EventHandler<Command> MsgReceived;

        public string TargetID { get; private set; }
        public string ID { get; private set; }
        public IPCHelper(string id, string targetId)
        {
            TargetID = targetId;
            ID = id;

            _messageBus = new TinyMessageBus("livewallpaper");
            _messageBus.MessageReceived += _messageBus_MessageReceived;
        }

        private void _messageBus_MessageReceived(object sender, TinyMessageReceivedEventArgs e)
        {
            var msg = JsonConvert.DeserializeObject<Command>(Encoding.UTF8.GetString(e.Message));
            if (msg.TargetID != null && msg.TargetID != ID)
                return;

            MsgReceived?.Invoke(this, msg);
            _messages.Enqueue(msg);
        }

        public void Dispose()
        {
            _messageBus.MessageReceived -= _messageBus_MessageReceived;
            _messageBus?.Dispose();
            _messageBus = null;
        }

        public Task Send<T>(T command)
        {
            Command realCommand = new Command();
            realCommand.CommandFullName = typeof(T).FullName;
            realCommand.Parameter = JsonConvert.SerializeObject(command);
            realCommand.TargetID = TargetID;
            realCommand.FromId = ID;
            var json = JsonConvert.SerializeObject(realCommand);
            return _messageBus.PublishAsync(Encoding.UTF8.GetBytes(json));
        }

        internal async Task<R> SendAndWait<T, R>(T command, int timeOut = 1000 * 60)
        {
            R r = default;
            Task wait = Task.Run((async () =>
            {
                r = await Wait<R>(timeOut);
            }));

            _ = Send(command);
            await Task.WhenAny(wait, Task.Delay(timeOut));
            return r;
        }

        internal async Task<T> Wait<T>(int timeOut = 1000 * 30)
        {
            DateTime startTime = DateTime.Now;
            while (_messages.TryDequeue(out var msg) || DateTime.Now - startTime < TimeSpan.FromSeconds(timeOut))
            {
                if (msg == null)
                    //还没有消息多等n毫秒
                    await Task.Delay(100);
                else if (msg.CommandFullName == typeof(T).FullName)
                {
                    var result = JsonConvert.DeserializeObject<T>(msg.Parameter.ToString());
                    return result;
                }

                await Task.Delay(100);
            }
            return default;
        }
    }
}
