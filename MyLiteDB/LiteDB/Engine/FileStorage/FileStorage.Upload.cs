﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace LiteDB
{
    public partial class FileStorage
    {
        /// <summary>
        /// Insert or update a file content inside datafile
        /// </summary>
        public FileEntry Upload(string key, string filename, Stream stream, NameValueCollection metadata = null)
        {
            if (string.IsNullOrEmpty(key)) throw new ArgumentNullException("key");
            if (string.IsNullOrEmpty(filename)) throw new ArgumentNullException("stream");
            if (stream == null) throw new ArgumentNullException("stream");

            if(!Regex.IsMatch(key, @"^[^\.<>\\/|:""*][^<>\\/|:""*]*(/[^\.<>\\/|:""*][^<>\\/|:""*]*)*$"))
                throw new ArgumentException("Invalid key format. Use key as path/to/file/filename.ext");

            // try to find _files collection - if not found, create a new one inside a transaction and commit changes
            if (_engine.Collections.Get("_files") == null)
            {
                _engine.BeginTrans();
                _engine.Collections.Add("_files");
                _engine.Commit();
            }

            // find document and convert to entry (or create a new one)
            var doc = _col.FindById(key);

            var entry = doc == null ? new FileEntry(key, filename, metadata) : new FileEntry(doc);

            // storage do not use cache - read/write pages directly from disk
            // so, transaction is not allowed. 
            // clear cache to garantee that are do not have dirty pages

            if (_engine.Transaction.IsInTransaction)
                throw new LiteException("Files can´t be used inside a transaction.");

            _engine.Transaction.Begin();

            // at this point, all cache pages are the same in disk, so I can use any of them

            try
            {
                // not found? then insert
                if (doc == null)
                {
                    var page = _engine.Data.NextPage(null);

                    entry.PageID = page.PageID;
                    entry.Length = _engine.Data.StoreStreamData(page, stream);

                    _col.Insert(entry.ToBsonDocument());
                }
                else
                {
                    var page = _engine.Disk.ReadPage<ExtendPage>(entry.PageID);

                    entry.Length = _engine.Data.StoreStreamData(page, stream);
                    entry.UploadDate = DateTime.Now;
                    entry.Metadata = metadata ?? entry.Metadata;

                    _col.Update(entry.ToBsonDocument());
                }

                _engine.Transaction.Commit();

            }
            catch (Exception ex)
            {
                _engine.Transaction.Rollback();
                throw ex;
            }

            return entry;
        }

        public FileEntry Upload(string key, Stream stream, NameValueCollection metadata = null)
        {
            return this.Upload(key, Path.GetFileName(key), stream, metadata);
        }

        public FileEntry Upload(string key, string filename, NameValueCollection metadata = null)
        {
            using (var stream = new FileStream(filename, FileMode.Open, FileAccess.Read))
            {
                return this.Upload(key, filename, stream, metadata);
            }
        }

        /// <summary>
        /// Updates a file entry on storage - do not change file content, only metadata will be update
        /// </summary>
        public bool Update(FileEntry file)
        {
            if (file == null) throw new ArgumentNullException("file");

            return _col.Update(file.ToBsonDocument());
        }
    }
}
