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
    /// Represent a Bson Value used in BsonDocument
    /// </summary>
    public class BsonValue
    {
        internal JToken Value = null;

        internal BsonValue()
        {
            this.Value = new JObject();
        }

        internal BsonValue(object value)
        {
            this.Value = JToken.FromObject(value);
        }

        internal BsonValue(JToken value)
        {
            this.Value = value;
        }

        public BsonType Type
        {
            get
            {
                switch (this.Value.Type)
                {
                    case JTokenType.Array: return BsonType.Array;
                    case JTokenType.Boolean: return BsonType.Boolean;
                    case JTokenType.Date: return BsonType.DateTime;
                    case JTokenType.Guid: return BsonType.Guid;
                    case JTokenType.Null: return BsonType.Null;
                    case JTokenType.String: return BsonType.String;
                    case JTokenType.Integer: return BsonType.Integer;
                    case JTokenType.Float: return BsonType.Decimal;
                    default: return BsonType.Object;
                }
            }
        }

        #region "this" operators for BsonObject/BsonArray

        public BsonValue this[string name]
        {
            get
            {
                if(this.Type != BsonType.Object) throw new LiteDBException("Value is not an object");

                var obj = (JObject)this.Value;
                return new BsonValue(obj.GetValue(name));
            }
            set
            {
                if (this.Type != BsonType.Object) throw new LiteDBException("Value is not an object");

                var obj = (JObject)this.Value;
                obj[name] = value.Value;
            }
        }

        public BsonValue this[int index]
        {
            get
            {
                if(this.Type != BsonType.Array) throw new LiteDBException("Value is not an array");

                var array = (JArray)this.Value;
                return new BsonValue(array.ElementAt(index));
            }
            set
            {
                if(this.Type != BsonType.Array) throw new LiteDBException("Value is not an array");

                var array = (JArray)Value;
                array[index] = value.Value;
            }
        }

        /// <summary>
        /// Same as doc[key] = value but with fluent api. Returns same object
        /// </summary>
        public BsonValue Append(string key, object value)
        {
            this[key] = new BsonValue(value);
            return this;
        }

        #endregion

        #region Convert types

        public BsonArray AsArray
        {
            get 
            {
                if (this.Type != BsonType.Array) throw new LiteDBException("Value is not an array");
                return new BsonArray((JArray)this.Value);
            }
        }

        public BsonObject AsObject
        {
            get
            {
                if(this.Type != BsonType.Object) throw new LiteDBException("Value is not an object");

                return new BsonObject((JObject)this.Value);
            }
        }

        public string AsString
        {
            get { return this.Type == BsonType.String ? this.Value.Value<string>() : null; }
        }

        public decimal AsDecimal
        {
            get { return this.Type == BsonType.Decimal ? this.Value.Value<decimal>() : 0; }
        }

        public int AsInt
        {
            get { return this.Type == BsonType.Integer ? this.Value.Value<int>() : 0; }
        }

        public bool AsBoolean
        {
            get { return this.Type == BsonType.Boolean ? this.Value.Value<bool>() : false; }
        }

        public DateTime AsDateTime
        {
            get { return this.Type == BsonType.DateTime ? this.Value.Value<DateTime>() : DateTime.MinValue; }
        }

        public Guid AsGuid
        {
            get { return this.Type == BsonType.Guid ? this.Value.Value<Guid>() : Guid.Empty; }
        }

        public T As<T>()
        {
            return this.Value.ToObject<T>();
        }

        #endregion

        #region IsTypes

        public bool IsNull
        {
            get { return this.Type == BsonType.Null; }
        }

        public bool IsArray
        {
            get { return this.Type == BsonType.Array; }
        }

        public bool IsInteger
        {
            get { return this.Type == BsonType.Integer; }
        }

        public bool IsDecimal
        {
            get { return this.Type == BsonType.Decimal; }
        }

        public bool IsGuid
        {
            get { return this.Type == BsonType.Guid; }
        }

        public bool IsDateTime
        {
            get { return this.Type == BsonType.DateTime; }
        }

        public bool IsBoolean
        {
            get { return this.Type == BsonType.Boolean; }
        }

        public bool IsObject
        {
            get { return this.Type == BsonType.Object; }
        }

        #endregion

        #region Operators

        public static implicit operator string(BsonValue value)
        {
            return value.AsString;
        }

        public static implicit operator BsonValue(string value)
        {
            return new BsonValue(value);
        }

        public static implicit operator decimal(BsonValue value)
        {
            return value.AsDecimal;
        }

        public static implicit operator BsonValue(decimal value)
        {
            return new BsonValue(value);
        }

        public static implicit operator int(BsonValue value)
        {
            return value.AsInt;
        }

        public static implicit operator BsonValue(int value)
        {
            return new BsonValue(value);
        }

        public static implicit operator bool(BsonValue value)
        {
            return value.AsBoolean;
        }

        public static implicit operator BsonValue(bool value)
        {
            return new BsonValue(value);
        }

        public static implicit operator DateTime(BsonValue value)
        {
            return value.AsDateTime;
        }

        public static implicit operator BsonValue(DateTime value)
        {
            return new BsonValue(value);
        }

        public static implicit operator Guid(BsonValue value)
        {
            return value.AsGuid;
        }

        public static implicit operator BsonValue(Guid value)
        {
            return new BsonValue(value);
        }

        public override string ToString()
        {
            return this.AsString;
        }

        #endregion
    }
}
