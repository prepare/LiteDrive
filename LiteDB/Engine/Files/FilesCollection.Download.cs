using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace LiteDB
{
    public partial class FilesCollection
    {
        /// <summary>
        /// Copy all file content to a steam
        /// </summary>
        public void Download(string key, Stream stream)
        {
            if (string.IsNullOrEmpty(key)) throw new ArgumentNullException("key");
            if (stream == null) throw new ArgumentNullException("stream");

            using (var s = this.OpenRead(key))
            {
                if (s == null) throw new LiteException("File not found");

                s.CopyTo(stream);
            }
        }

        /// <summary>
        /// Load data inside storage and copy to stream
        /// </summary>
        public LiteFileStream OpenRead(string key)
        {
            if (string.IsNullOrEmpty(key)) throw new ArgumentNullException("key");

            var doc = _col.FindById(key);

            if (doc == null) return null;

            return this.OpenRead(new FileEntry(doc));
        }

        /// <summary>
        /// Load data inside storage and copy to stream
        /// </summary>
        internal LiteFileStream OpenRead(FileEntry entry)
        {
            if (entry == null) throw new ArgumentNullException("entry");

            return new LiteFileStream(_engine, entry);
        }
    }
}
