using System;
using System.IO;
using Microsoft.Toolkit.Win32.UI.Controls.Interop.WinRT;

namespace LiveWallpaperEngineRender
{
    internal class StreamUriResolver : IUriToStreamResolver
    {
        public Stream UriToStream(Uri uri)
        {
            var stream = File.OpenRead(uri.LocalPath);
            return stream;
        }
    }
}