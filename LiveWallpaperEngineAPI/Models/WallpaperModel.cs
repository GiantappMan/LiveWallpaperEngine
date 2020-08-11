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

    public class WallpaperInfo
    {
        public string Description { get; set; }
        public string File { get; set; }
        public string Preview { get; set; }
        public string Tags { get; set; }
        public string Title { get; set; }
        public string Type { get; set; }
        public string Visibility { get; set; }
    }

    public enum WallpaperType
    {
        Video,
        Image,
        Web,
        Exe
    }

    public class WallpaperModel
    {
        /// <summary>
        /// 是否支持鼠标事件，exe和web才行。其他类型设置无效
        /// </summary>
        public bool EnableMouseEvent { get; set; } = true;
        public WallpaperType? Type { get; set; }
        public bool IsEventWallpaper
        {
            get
            {
                var r = EnableMouseEvent &&
                    (
                    Type.Value == WallpaperType.Exe ||
                    Type.Value == WallpaperType.Web
                    );
                return r;
            }
        }
        public bool IsPaused { get; set; }
        public bool IsStopedTemporary { get; set; }
        public string Path { get; set; }
    }

    public class WallpaperModelInfo : WallpaperModel
    {
        public WallpaperInfo Info { get; set; }
    }
}
