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
            : base(new List<BsonValue>())
        {
        }

        public BsonArray(List<BsonValue> array)
            : base(array)
        {
        }

        public void Add(BsonValue value)
        {
            this.RawValue.Add(value);
        }

        public void Remove(int index)
        {
            this.RawValue.RemoveAt(index);
        }

        public int Length
        {
            get
            {
                return this.RawValue.Count;
            }
        }

        public new List<BsonValue> RawValue
        {
            get
            {
                return (List<BsonValue>)base.RawValue;
            }
        }
    }
}
