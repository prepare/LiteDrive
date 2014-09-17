using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;

namespace LiteDB
{
    /// <summary>
    /// Represets a file inside files collection
    /// </summary>
    public class FileEntry
    {
        public string Key { get; private set; }
        public string Filename { get; private set; }
        public string MimeType { get; private set; }
        public int Length { get; internal set; }
        public DateTime UploadDate { get; internal set; }
        public Dictionary<string, string> Metadata { get; internal set; }

        internal uint PageID { get; set; }

        internal FileEntry(string key, string filename, Dictionary<string, string> metadata)
        {
            this.Key = key;
            this.Filename = filename;
            this.MimeType = MimeTypeConverter.GetMimeType(this.Filename);
            this.Metadata = metadata ?? new Dictionary<string, string>();
            this.UploadDate = DateTime.Now;

            this.PageID = uint.MaxValue;
        }

        internal FileEntry(BsonDocument doc)
        {
            this.Key = doc["Key"].AsString;
            this.Filename = doc["Filename"].AsString;
            this.MimeType = doc["MimeType"].AsString;
            this.Length = doc["Length"].AsInt;
            this.UploadDate = doc["UploadDate"].AsDateTime;
            this.Metadata = doc["Metadata"].As<Dictionary<string, string>>();

            this.PageID = (uint)doc["PageID"].As<uint>();
        }

        internal BsonDocument ToBson()
        {
            var doc = new BsonDocument();

            doc["Key"] = new BsonValue(this.Key);
            doc["Filename"] = new BsonValue(this.Filename);
            doc["MimeType"] = new BsonValue(this.MimeType);
            doc["Length"] = new BsonValue(this.Length);
            doc["UploadDate"] = new BsonValue(this.UploadDate);
            doc["Metadata"] = new BsonObject(this.Metadata);
            doc["PageID"] = new BsonValue(this.PageID);

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
