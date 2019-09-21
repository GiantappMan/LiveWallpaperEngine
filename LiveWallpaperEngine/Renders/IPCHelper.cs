using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Text;
using System.Threading.Tasks;
using TinyIpc.Messaging;

namespace LiveWallpaperEngine.Renders
{
    #region ToServerCommands
    public struct RenderInitlized
    {
        public IntPtr Handle { get; set; }
    }

    #endregion

    #region ToClientCommands

    public struct LaunchWallpaper
    {
        public int PID { get; set; }
        public string ServerID { get; set; }
    }

    #endregion

    public class Command<T>
    {
        // 接收人id，如果为空。都可以接收
        public string TargetID { get; set; }
        // 命令标识
        public string CommandFullName { get; set; }
        public T Parameter { get; set; }
    }

    class IPCHelper : IDisposable
    {
        TinyMessageBus _messageBus;
        ConcurrentQueue<Command<object>> _messages = new ConcurrentQueue<Command<object>>();

        public string ID { get; private set; }
        public IPCHelper()
        {
            ID = Guid.NewGuid().ToString();
            _messageBus = new TinyMessageBus("livewallpaper");
            _messageBus.MessageReceived += _messageBus_MessageReceived;
        }

        private void _messageBus_MessageReceived(object sender, TinyMessageReceivedEventArgs e)
        {
            var msg = JsonConvert.DeserializeObject<Command<object>>(Encoding.UTF8.GetString(e.Message));
            if (msg.TargetID != null && msg.TargetID != ID)
                return;

            _messages.Enqueue(msg);
        }

        public void Dispose()
        {
            _messageBus?.Dispose();
            _messageBus = null;
        }

        public Task Send<T>(T command)
        {
            Command<T> realCommand = new Command<T>();
            realCommand.CommandFullName = typeof(T).FullName;
            realCommand.Parameter = command;
            var json = JsonConvert.SerializeObject(realCommand);
            return _messageBus.PublishAsync(Encoding.UTF8.GetBytes(json));
        }

        internal async Task<R> SendAndWait<T, R>(T command, int timeOut = 1000 * 60)
        {
            R r = default;
            Task wait = Task.Run((async () =>
              {
                  r = await Wait<R>();
              }));

            _ = Send(command);
            await Task.WhenAny(wait, Task.Delay(timeOut));
            return r;
        }

        internal async Task<T> Wait<T>()
        {
            while (_messages.TryDequeue(out var msg) || true)
            {
                if (msg == null)
                    //还没有消息多等n毫秒
                    await Task.Delay(100);
                else if (msg.CommandFullName == typeof(T).FullName)
                    return JsonConvert.DeserializeObject<T>(msg.Parameter.ToString());

                await Task.Delay(100);
            }
            return default;
        }
    }
}
