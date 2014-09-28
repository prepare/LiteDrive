using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace LiteDB
{
    public class BsonSerializer : ISerializer<byte[]>
    {
        static BsonSerializer()
        {
            fastBinaryJSON.BJSON.Parameters.UseExtensions = false;
            //fastBinaryJSON.BJSON.Parameters.IgnoreAttributes.Clear();
            //fastBinaryJSON.BJSON.Parameters.IgnoreAttributes.Add(typeof(BsonIgnoreAttribute));
        }

        public byte[] FromDocument(BsonDocument doc)
        {
            return null;
        }

        public BsonDocument ToDocument(byte[] data)
        {
            return null;
        }
    }
}
