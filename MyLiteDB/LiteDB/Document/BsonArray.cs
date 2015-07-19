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

        public BsonArray(List<object> array)
            : base(array)
        {
        }

        public void Add(BsonValue value)
        {
            this.RawValue.Add(value.RawValue);
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

        public new List<object> RawValue
        {
            get
            {
                return (List<object>)base.RawValue;
            }
        }
    }
}
