using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;

namespace LiteDB
{
    /// <summary>
    /// Represets a file inside storage collection
    /// </summary>
    public class FileEntry
    {
        public string Key { get; private set; }
        public string Filename { get; private set; }
        public string MimeType { get; private set; }
        public int Length { get; internal set; }
        public DateTime UploadDate { get; internal set; }
        public NameValueCollection Metadata { get; internal set; }

        internal uint PageID { get; set; }

        internal FileEntry(string key, string filename, NameValueCollection metadata)
        {
            this.Key = key;
            this.Filename = filename;
            this.MimeType = MimeTypeConverter.GetMimeType(this.Filename);
            this.Metadata = metadata ?? new NameValueCollection();
            this.UploadDate = DateTime.Now;

            this.PageID = uint.MaxValue;
        }

        internal FileEntry(BsonDocument doc)
        {
            this.Key = (string)doc.Id;
            this.Filename = doc["Filename"].AsString;
            this.MimeType = doc["MimeType"].AsString;
            this.Length = doc["Length"].AsInt;
            this.UploadDate = doc["UploadDate"].AsDateTime;
            this.Metadata = new NameValueCollection();
            this.Metadata.ParseQueryString(doc["Metadata"].AsString);
            this.PageID = doc["PageID"].AsUInt;
        }

        internal BsonDocument ToBsonDocument()
        {
            var doc = new BsonDocument();

            doc.Id = this.Key;
            doc["Filename"] = this.Filename;
            doc["MimeType"] = this.MimeType;
            doc["Length"] = this.Length;
            doc["UploadDate"] = this.UploadDate;
            doc["Metadata"] = this.Metadata.ToString();
            doc["PageID"] = this.PageID;

            return doc;
        }

        /// <summary>
        /// Open file stream to read from database
        /// </summary>
        public LiteFileStream OpenRead(LiteEngine db)
        {
            return new LiteFileStream(db, this);
        }

        /// <summary>
        /// Save file content to a external file
        /// </summary>
        public void SaveAs(LiteEngine db, string filename, bool overwritten = true)
        {
            using (var file = new FileStream(filename, overwritten ? FileMode.Create : FileMode.CreateNew))
            {
                this.OpenRead(db).CopyTo(file);
            }
        }
    }
}
