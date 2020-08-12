using System;
using System.Collections.Generic;
using System.Text;

namespace Giantapp.LiveWallpaper.Engine
{
    public class ProgressChangedArgs : EventArgs
    {
        public enum Type
        {
            Downloading,
            Unpacking
        }
        public Type ActionType { get; set; }
        public bool Completed { get; set; }
        public string Error { get; set; }
        public float ProgressPercentage { get; set; }
        public string Path { get; set; }
    }
    //包含所有的错误类型
    public enum ErrorType
    {
        None,
        NoPlayer,
        DownloadFailed,
        NoRender,
        Canceled,
        Uninitialized,
        Busy,
        Exception
    }
    public class BaseApiResult
    {
        public bool Ok { get; set; }
        public ErrorType Error { get; set; }
        public string Message { get; set; }

        internal static BaseApiResult BusyState()
        {
            return new BaseApiResult() { Ok = false, Error = ErrorType.Busy };
        }

        internal static BaseApiResult ExceptionState(Exception ex)
        {
            return new BaseApiResult() { Ok = false, Error = ErrorType.Exception, Message = ex.Message };
        }

        internal static BaseApiResult ErrorState(ErrorType type, string msg = null)
        {
            return new BaseApiResult() { Ok = false, Error = type, Message = msg };
        }

        internal static BaseApiResult SuccessState()
        {
            return new BaseApiResult() { Ok = true };
        }
    }

    public class BaseApiResult<T> : BaseApiResult
    {
        public T Data { get; set; }
    }

    public class ShowWallpaperResult : BaseApiResult
    {
        internal List<RenderInfo> RenderInfos { get; set; }
    }

    public class RenderProcess
    {
        public IntPtr HostHandle { get; set; }
        public IntPtr ReceiveMouseEventHandle { get; set; }
        public int PId { get; set; }
    }

    public class RenderInfo : RenderProcess
    {
        public RenderInfo()
        {

        }
        public RenderInfo(RenderProcess p)
        {
            HostHandle = p.HostHandle;
            ReceiveMouseEventHandle = p.ReceiveMouseEventHandle;
            PId = p.PId;
        }
        public WallpaperModel Wallpaper { get; set; }
        public string Screen { get; set; }
    }

    public enum WallpaperType
    {
        Video,
        Image,
        Web,
        Exe
    }
    public class WallpaperOption
    {
        /// <summary>
        /// 是否支持鼠标事件，exe和web才行。其他类型设置无效
        /// </summary>
        public bool EnableMouseEvent { get; set; } = true;
    }

    public class WallpaperRunningData
    {
        /// <summary>
        /// 壁纸所在文件夹
        /// </summary>
        public string Dir { get; set; }
        public bool IsPaused { get; set; }
        public bool IsStopedTemporary { get; set; }
    }
    public class WallpaperProjectInfo
    {
        public string Description { get; set; }
        public string Title { get; set; }
        public string File { get; set; }
        public string Preview { get; set; }
        public string Type { get; set; }
        public string Visibility { get; set; }
        public List<string> Tags { get; set; }
    }

    public class WallpaperModel
    {
        public WallpaperOption Option { get; set; } = new WallpaperOption();
        public WallpaperRunningData RunningData { get; set; } = new WallpaperRunningData();
        public WallpaperProjectInfo Info { get; set; } = new WallpaperProjectInfo();
        public WallpaperType? Type { get; set; }
        public string Path { get; set; }
    }
}
