using System;
using System.Collections.Generic;
using System.Text;

namespace Giantapp.LiveWallpaper.Engine
{
    //public static class ConstWallpaperTypes
    //{
    //    public static Dictionary<WallpaperType, string[]> DefinedType = new Dictionary<WallpaperType, string[]>()
    //    {
    //        { WallpaperType.Exe,new string[]{".exe" }},
    //        { WallpaperType.Video,new string[]{".mp4", ".flv", ".blv", ".avi", ".gif" }},
    //        { WallpaperType.Image,new string[]{".jpg", ".png", ".jpeg", ".bmp" }},
    //        { WallpaperType.Web,new string[]{".html", ".htm" }},
    //    };
    //}
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
        public WallpaperType? Type { get; set; }
        public string Path { get; set; }
    }
    public class WallpaperModelInfo : WallpaperModel
    {
        public WallpaperInfo Info { get; set; }
    }
}
