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
        {
            if (obj == null) throw new ArgumentNullException("obj");

            // if obj is BsonDocument, just get JToken Value instance. Do not create a copy of JToken
            this.Value = obj is BsonDocument ? ((BsonDocument)obj).Value : JObject.FromObject(obj);
        }

        public BsonDocument(string json)
        {
            if (string.IsNullOrEmpty(json)) throw new ArgumentNullException("json");

            this.Value = JObject.Parse(json);
        }

        public BsonDocument(byte[] data)
        {
            if (data == null || data.Length == 0) throw new ArgumentNullException("data");

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

                    var bytes = stream.ToArray();

                    if (bytes.Length > BsonDocument.MAX_DOCUMENT_SIZE)
                        throw new LiteException("Object exceed limit of " + Math.Truncate(BsonDocument.MAX_DOCUMENT_SIZE / 1024m) + " Kb");

                    return bytes;
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

        /// <summary>
        /// Create a deep clone of a BsonDocument
        /// </summary>
        public BsonDocument Clone()
        {
            var doc = new BsonDocument();
            doc.Value = this.Value.DeepClone();

            return doc;
        }
    }
}
