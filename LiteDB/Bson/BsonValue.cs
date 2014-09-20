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
        public BsonType Type { get; private set; }
        public virtual object RawValue { get; private set; }

        #region Constructor

        public BsonValue()
        {
            this.Type = BsonType.Null;
        }

        public BsonValue(List<object> value)
        {
            this.Type = BsonType.Array;
            this.RawValue = value;
        }

        public BsonValue(Dictionary<string, object> value)
        {
            this.Type = BsonType.Object;
            this.RawValue = value;
        }

        public BsonValue(byte value)
        {
            this.Type = BsonType.Byte;
            this.RawValue = value;
        }

        public BsonValue(char value)
        {
            this.Type = BsonType.Char;
            this.RawValue = value;
        }

        public BsonValue(bool value)
        {
            this.Type = BsonType.Boolean;
            this.RawValue = value;
        }

        public BsonValue(string value)
        {
            this.Type = value == null ? BsonType.Null : BsonType.String;
            this.RawValue = value;
        }

        public BsonValue(short value)
        {
            this.Type = BsonType.Short;
            this.RawValue = value;
        }

        public BsonValue(int value)
        {
            this.Type = BsonType.Int;
            this.RawValue = value;
        }

        public BsonValue(long value)
        {
            this.Type = BsonType.Long;
            this.RawValue = value;
        }

        public BsonValue(ushort value)
        {
            this.Type = BsonType.UShort;
            this.RawValue = value;
        }

        public BsonValue(uint value)
        {
            this.Type = BsonType.UInt;
            this.RawValue = value;
        }

        public BsonValue(ulong value)
        {
            this.Type = BsonType.ULong;
            this.RawValue = value;
        }

        public BsonValue(float value)
        {
            this.Type = BsonType.Float;
            this.RawValue = value;
        }

        public BsonValue(double value)
        {
            this.Type = BsonType.Double;
            this.RawValue = value;
        }

        public BsonValue(decimal value)
        {
            this.Type = BsonType.Decimal;
            this.RawValue = value;
        }

        public BsonValue(DateTime value)
        {
            this.Type = BsonType.DateTime;
            this.RawValue = value;
        }

        public BsonValue(Guid value)
        {
            this.Type = BsonType.Guid;
            this.RawValue = value;
        }

        public BsonValue(BsonValue value)
        {
            this.Type = value.Type;
            this.RawValue = value.RawValue;
        }

        public BsonValue(object value)
        {
            this.RawValue = value;

            if (value == null) this.Type = BsonType.Null;
            else if (value is List<object>) this.Type = BsonType.Array;
            else if (value is Dictionary<string, object>) this.Type = BsonType.Object;
            else if (value is byte) this.Type = BsonType.Byte;
            else if (value is char) this.Type = BsonType.Char;
            else if (value is bool) this.Type = BsonType.Boolean;
            else if (value is string) this.Type = BsonType.String;
            else if (value is short) this.Type = BsonType.Short;
            else if (value is int) this.Type = BsonType.Int;
            else if (value is long) this.Type = BsonType.Long;
            else if (value is ushort) this.Type = BsonType.UShort;
            else if (value is uint) this.Type = BsonType.UInt;
            else if (value is ulong) this.Type = BsonType.ULong;
            else if (value is float) this.Type = BsonType.Float;
            else if (value is double) this.Type = BsonType.Double;
            else if (value is decimal) this.Type = BsonType.Decimal;
            else if (value is DateTime) this.Type = BsonType.DateTime;
            else if (value is Guid) this.Type = BsonType.Guid;
            else if (value is BsonValue)
            {
                this.Type = ((BsonValue)value).Type;
                this.RawValue = ((BsonValue)value).RawValue;
            }
            else throw new InvalidCastException("Value is not a valid data type");
        }

        #endregion

        #region "this" operators for BsonObject/BsonArray

        public BsonValue this[string name]
        {
            get
            {
                return new BsonValue(this.AsObject.RawValue[name]);
            }
            set
            {
                this.AsObject.RawValue[name] = value;
            }
        }

        public BsonValue this[int index]
        {
            get
            {
                return new BsonValue(this.AsArray.RawValue.ElementAt(index));
            }
            set
            {
                this.AsArray.RawValue[index] = value.RawValue;
            }
        }

        #endregion

        #region Convert types

        public BsonArray AsArray
        {
            get 
            {
                if (this.Type != BsonType.Array) throw new LiteException("Value is not an array");
                return new BsonArray((List<object>)this.RawValue);
            }
        }

        public BsonObject AsObject
        {
            get
            {
                if(this.Type != BsonType.Object) throw new LiteException("Value is not an object");
                return new BsonObject((Dictionary<string, object>)this.RawValue);
            }
        }

        public byte AsByte
        {
            get { return this.Type == BsonType.Byte ? (byte)this.RawValue : (byte)0; }
            set { this.Type = BsonType.Byte; this.RawValue = value; }
        }

        public char AsChar
        {
            get { return this.Type == BsonType.Char ? (char)this.RawValue : '\0'; }
            set { this.Type = BsonType.Char; this.RawValue = value; }
        }

        public bool AsBoolean
        {
            get { return this.Type == BsonType.Boolean ? (bool)this.RawValue : false; }
            set { this.Type = BsonType.Boolean; this.RawValue = value; }
        }

        public string AsString
        {
            get { return this.Type == BsonType.String ? (string)this.RawValue : null; }
            set { this.Type = value == null ? BsonType.Null : BsonType.String; this.RawValue = value; }
        }

        public short AsShort
        {
            get { return this.Type == BsonType.Short ? (short)this.RawValue : (short)0; }
            set { this.Type = BsonType.Short; this.RawValue = value; }
        }

        public int AsInt
        {
            get { return this.Type == BsonType.Int ? (int)this.RawValue : 0; }
            set { this.Type = BsonType.Int; this.RawValue = value; }
        }

        public long AsLong
        {
            get { return this.Type == BsonType.Long ? (long)this.RawValue : 0; }
            set { this.Type = BsonType.Long; this.RawValue = value; }
        }

        public ushort AsUShort
        {
            get { return this.Type == BsonType.UShort ? (ushort)this.RawValue : (ushort)0; }
            set { this.Type = BsonType.UShort; this.RawValue = value; }
        }

        public uint AsUInt
        {
            get { return this.Type == BsonType.UInt ? (uint)this.RawValue : 0; }
            set { this.Type = BsonType.UInt; this.RawValue = value; }
        }

        public ulong AsULong
        {
            get { return this.Type == BsonType.ULong ? (ulong)this.RawValue : 0; }
            set { this.Type = BsonType.ULong; this.RawValue = value; }
        }

        public float AsFloat
        {
            get { return this.Type == BsonType.Float ? (float)this.RawValue : 0; }
            set { this.Type = BsonType.Float; this.RawValue = value; }
        }

        public double AsDouble
        {
            get { return this.Type == BsonType.Double ? (double)this.RawValue : 0; }
            set { this.Type = BsonType.Double; this.RawValue = value; }
        }

        public decimal AsDecimal
        {
            get { return this.Type == BsonType.Decimal ? (decimal)this.RawValue : 0; }
            set { this.Type = BsonType.Decimal; this.RawValue = value; }
        }

        public DateTime AsDateTime
        {
            get { return this.Type == BsonType.DateTime ? (DateTime)this.RawValue : DateTime.MinValue; }
            set { this.Type = BsonType.DateTime; this.RawValue = value; }
        }

        public Guid AsGuid
        {
            get { return Type == BsonType.Guid ? (Guid)this.RawValue : Guid.Empty; }
            set { this.Type = BsonType.Guid; this.RawValue = value; }
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

        public bool IsObject
        {
            get { return this.Type == BsonType.Object; }
        }

        #endregion

        #region Operators

        public static implicit operator byte(BsonValue value)
        {
            return value.AsByte;
        }

        public static implicit operator BsonValue(byte value)
        {
            return new BsonValue(value);
        }

        public static implicit operator char(BsonValue value)
        {
            return value.AsChar;
        }

        public static implicit operator BsonValue(char value)
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

        public static implicit operator string(BsonValue value)
        {
            return value.AsString;
        }

        public static implicit operator BsonValue(string value)
        {
            return new BsonValue(value);
        }

        public static implicit operator short(BsonValue value)
        {
            return value.AsShort;
        }

        public static implicit operator BsonValue(short value)
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

        public static implicit operator long(BsonValue value)
        {
            return value.AsLong;
        }

        public static implicit operator BsonValue(long value)
        {
            return new BsonValue(value);
        }

        public static implicit operator ushort(BsonValue value)
        {
            return value.AsUShort;
        }

        public static implicit operator BsonValue(ushort value)
        {
            return new BsonValue(value);
        }

        public static implicit operator uint(BsonValue value)
        {
            return value.AsUInt;
        }

        public static implicit operator BsonValue(uint value)
        {
            return new BsonValue(value);
        }

        public static implicit operator ulong(BsonValue value)
        {
            return value.AsULong;
        }

        public static implicit operator BsonValue(ulong value)
        {
            return new BsonValue(value);
        }

        public static implicit operator float(BsonValue value)
        {
            return value.AsFloat;
        }

        public static implicit operator BsonValue(float value)
        {
            return new BsonValue(value);
        }

        public static implicit operator double(BsonValue value)
        {
            return value.AsDouble;
        }

        public static implicit operator BsonValue(double value)
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

        #endregion
    }
}
