using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace LiteDB
{
    /// <summary>
    /// Represent a document schemeless to use in collections. Based on Dictionary<string, object>
    /// </summary>
    public class BsonDocument : BsonValue
    {
        public const int MAX_DOCUMENT_SIZE = 256 * 124;

        public BsonDocument()
            : base(new Dictionary<string, object>())
        {
        }

        public BsonDocument(object obj)
            : this()
        {
            //this.Append(anonymousObject);
        }

        public BsonDocument(string json)
        {
        }

        public BsonDocument(byte[] data)
        {
        }

        public byte[] ToBson()
        {
            return null;
        }

        public object GetFieldValue(string key)
        {
            return null;
        }
    }
}
