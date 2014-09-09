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
        public const int MAX_DOCUMENT_SIZE = 256 * 1024; // limits in 256 max document size to avoid large documents, memory usage and slow performance

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
            Newtonsoft.Json.Linq.JObject o;
            Newtonsoft.Json.Linq.JValue o;


        }

        public BsonDocument(byte[] data)
        {
        }

        public byte[] ToBson()
        {
            return null;
        }

        public string ToJson()
        {
            return "";
        }

        public T To<T>()
            where T : new()
        {
            return default(T);
        }

        public object GetFieldValue(string key)
        {
            return null;
        }
    }
}
