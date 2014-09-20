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
            : base(new Dictionary<string, object>())
        {
        }

        public BsonObject(Dictionary<string, object> obj)
            : base(obj)
        {
        }

        public new Dictionary<string, object> RawValue
        {
            get
            {
                return (Dictionary<string, object>)base.RawValue;
            }
        }

        public string[] Keys { get { return this.RawValue.Keys.ToArray(); } }
    }
}
