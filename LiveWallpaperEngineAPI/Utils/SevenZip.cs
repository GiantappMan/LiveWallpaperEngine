using SharpCompress.Archives;
using SharpCompress.Archives.SevenZip;
using SharpCompress.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Giantapp.LiveWallpaper.Engine.Utils
{
    class SevenZip
    {
        private readonly string _zipFile;

        public SevenZip(string zipPath)
        {
            _zipFile = zipPath;
        }

        internal bool Extract(string path)
        {
            using var archive = SevenZipArchive.Open(_zipFile);
            try
            {
                archive.CompressedBytesRead += Archive_CompressedBytesRead;
                foreach (var entry in archive.Entries.Where(entry => !entry.IsDirectory))
                {
                    entry.WriteToDirectory(path, new ExtractionOptions()
                    {
                        ExtractFullPath = true,
                        Overwrite = true
                    });
                }
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Extract ex:${ex}");
                return false;
            }
            finally
            {
                archive.CompressedBytesRead -= Archive_CompressedBytesRead;
            }
        }

        private void Archive_CompressedBytesRead(object sender, CompressedBytesReadEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine($"{e.CompressedBytesRead} {e.CurrentFilePartCompressedBytesRead}");
        }
    }
}
