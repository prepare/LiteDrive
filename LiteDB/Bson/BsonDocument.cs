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
            : base(new BsonObject())
        {
        }

        public BsonDocument(object obj)
            : this()
        {
            //this.Append(anonymousObject);
        }

        public BsonDocument(string json)
        {
            // JavaScriptSerializer a;
            // System.Web.Extensions

            JObject o = JObject.Parse(json);

            o.ToObject<BsonDocument>();

            


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
            if (typeof(T) == typeof(BsonDocument))
            {
                return (T)(object)this;
            }

            return default(T);
        }

        public object GetFieldValue(string key)
        {
            return null;
        }
    }
}
