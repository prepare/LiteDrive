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
            : base(new List<object>())
        {
        }

        public void Add(object value)
        {
            if (this.Type != BsonType.Array) throw new LiteDBException("Bson value is not an array");

            var array = (List<object>)this.RawValue;
            array.Add(value);
        }

        public void Add(BsonValue value)
        {
            if (this.Type != BsonType.Array) throw new LiteDBException("Bson value is not an array");

            var array = (List<object>)this.RawValue;
            array.Add(value.RawValue);
        }

        public void Remove(int index)
        {
            if (this.Type != BsonType.Array) throw new LiteDBException("Bson value is not an array");

            var array = (List<object>)this.RawValue;
            array.RemoveAt(index);

        }

        public int Length
        {
            get
            {
                if (this.Type != BsonType.Array) return this.Keys.Length;

                var array = (List<object>)this.RawValue;
                return array.Count;
            }
        }
    }
}
