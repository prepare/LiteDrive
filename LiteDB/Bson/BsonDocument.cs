using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
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
    /// Represent a document schemeless to use in collections. Based on JObject
    /// </summary>
    public class BsonDocument : BsonObject
    {
        public const int MAX_DOCUMENT_SIZE = 256 * 1024; // limits in 256 max document size to avoid large documents, memory usage and slow performance

        public BsonDocument()
            : base()
        {
        }

        public BsonDocument(object obj)
            : base(obj)
        {
        }

        public BsonDocument(string json)
        {
            this.Value = JObject.Parse(json);
        }

        public BsonDocument(byte[] data)
        {
            using (var reader = new BsonReader(new MemoryStream(data)))
            {
                this.Value = JObject.ReadFrom(reader);
            }
        }

        public byte[] ToBson()
        {
            using (var stream = new MemoryStream())
            {
                using (var writer = new BsonWriter(stream))
                {
                    this.Value.WriteTo(writer);

                    return stream.ToArray();
                }
            }
        }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this.Value);
        }

        public T To<T>()
            where T : new()
        {
            if (typeof(T) == typeof(BsonDocument))
            {
                return (T)(object)this;
            }

            return this.Value.ToObject<T>();
        }

        public object GetFieldValue(string field)
        {
            var value = this[field];

            if (value.Type == BsonType.Null) return null;
            
            return value.Value.ToObject(typeof(object));
        }
    }
}
