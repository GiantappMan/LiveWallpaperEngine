using SharpCompress.Archives;
using SharpCompress.Archives.SevenZip;
using SharpCompress.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Giantapp.LiveWallpaper.Engine.Utils
{

    class SevenZipUnzipProgressArgs : EventArgs
    {
        public float Progress { get; internal set; }
    }

    class SevenZip
    {
        private readonly string _zipFile;
        private long _total;
        private long _current;
        public event EventHandler<SevenZipUnzipProgressArgs> UnzipProgressChanged;


        public SevenZip(string zipPath)
        {
            _zipFile = zipPath;
        }

        internal void Extract(string path)
        {
            _total = 0;
            using var archive = SevenZipArchive.Open(_zipFile);
            try
            {
                archive.CompressedBytesRead += Archive_CompressedBytesRead;
                _total = archive.Entries.Count;
                foreach (var entry in archive.Entries.Where(entry => !entry.IsDirectory))
                {
                    entry.WriteToDirectory(path, new ExtractionOptions()
                    {
                        ExtractFullPath = true,
                        Overwrite = true
                    });
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                archive.CompressedBytesRead -= Archive_CompressedBytesRead;
            }
        }

        private void Archive_CompressedBytesRead(object sender, CompressedBytesReadEventArgs e)
        {
            _current++;
            //System.Diagnostics.Debug.WriteLine($"{e.CompressedBytesRead} {e.CurrentFilePartCompressedBytesRead}");
            //System.Diagnostics.Debug.WriteLine($"-----------{_tmp} {_total}");
            UnzipProgressChanged?.Invoke(this, new SevenZipUnzipProgressArgs()
            {
                Progress = (float)_current / _total
            });
        }
    }
}
