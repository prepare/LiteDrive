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
    public class BsonArray : BsonValue
    {
        public BsonArray()
            : base(new JArray())
        {
        }

        internal BsonArray(JArray array)
            : base(array)
        {
        }

        public void Add(BsonValue value)
        {
            var array = (JArray)this.Value;
            array.Add(value.Value);
        }

        public void Remove(int index)
        {
            var array = (JArray)this.Value;
            array.RemoveAt(index);
        }

        public int Length
        {
            get
            {
                var array = (JArray)this.Value;
                return array.Count;
            }
        }
    }
}
