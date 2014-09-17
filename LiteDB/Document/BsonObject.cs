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
    public class BsonObject : BsonValue
    {
        public BsonObject()
            : base(new JObject())
        {
        }

        public BsonObject(object obj)
            : base(JObject.FromObject(obj))
        {
        }
    }
}
