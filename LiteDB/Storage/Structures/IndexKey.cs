using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace LiteDB
{
    internal enum IndexDataType
    { 
        Null,
        // Int
        Byte,
        Int16,
        UInt16,
        Int32,
        UInt32,
        Int64,
        UInt64,
        // Decimal
        Single,
        Double,
        Decimal,
        // String
        String,
        // Others
        DateTime,
        Guid
    };

    /// <summary>
    /// Represent a index key value - can be a string, int, decimal, guid, ... It's persistable
    /// </summary>
    internal struct IndexKey : IComparable<IndexKey>
    {
        public const int MAX_LENGTH_SIZE = 255;

        public readonly object Value;

        public readonly IndexDataType Type;

        public readonly int Length;

        public IndexKey(object value)
        {
            this.Value = value;

            // null
            if (value == null) { this.Type = IndexDataType.Null; this.Length = 0; }

            // int
            else if (value is Byte) { this.Type = IndexDataType.Byte; this.Length = 1; }
            else if (value is Int16) { this.Type = IndexDataType.Int16; this.Length = 2; }
            else if (value is UInt16) { this.Type = IndexDataType.UInt16; this.Length = 2; }
            else if (value is Int32) { this.Type = IndexDataType.Int32; this.Length = 4; }
            else if (value is UInt32) { this.Type = IndexDataType.UInt32; this.Length = 4; }
            else if (value is Int64) { this.Type = IndexDataType.Int64; this.Length = 8; }
            else if (value is UInt64) { this.Type = IndexDataType.UInt64; this.Length = 8; }

            // decimal
            else if (value is Single) { this.Type = IndexDataType.Single; this.Length = 4; }
            else if (value is Double) { this.Type = IndexDataType.Double; this.Length = 8; }
            else if (value is Decimal) { this.Type = IndexDataType.Decimal; this.Length = 16; }

            // string
            else if (value is String) { this.Type = IndexDataType.String; this.Length = Encoding.UTF8.GetByteCount((string)Value) + 1 /* +1 = For String Length on store */; }

            // Others
            else if (value is DateTime) { this.Type = IndexDataType.DateTime; this.Length = 8; }
            else if (value is Guid) { this.Type = IndexDataType.Guid; this.Length = 16; }

            // if not found, exception
            else throw LiteException.IndexTypeNotSupport(value.GetType());

            // increase "Type" byte in length
            this.Length++;

            // withespace empty string == null
            if (this.Type == IndexDataType.String && ((string)value).Trim().Length == 0)
            {
                this.Value = null;
                this.Type = IndexDataType.Null;
                this.Length = 1;
            }

            // limit in 255 string byte
            if (this.Type == IndexDataType.String && this.Length > MAX_LENGTH_SIZE)
                throw LiteException.IndexKeyTooLong();
        }

        public int CompareTo(IndexKey other)
        {
            // first, compare Null values (null is always less than other type
            if (this.Type == IndexDataType.Null && other.Type == IndexDataType.Null) return 0;
            if (this.Type == IndexDataType.Null) return -1;
            if (other.Type == IndexDataType.Null) return 1;

            // if types are diferentes, convert to string
            if (this.Type != other.Type) return string.Compare(Value.ToString(), other.Value.ToString(), StringComparison.InvariantCultureIgnoreCase);

            // int
            if (this.Type == IndexDataType.Byte) return ((Byte)Value).CompareTo(other.Value);
            if (this.Type == IndexDataType.Int16) return ((Int16)Value).CompareTo(other.Value);
            if (this.Type == IndexDataType.UInt16) return ((UInt16)Value).CompareTo(other.Value);
            if (this.Type == IndexDataType.Int32) return ((Int32)Value).CompareTo(other.Value);
            if (this.Type == IndexDataType.UInt32) return ((UInt32)Value).CompareTo(other.Value);
            if (this.Type == IndexDataType.Int64) return ((Int64)Value).CompareTo(other.Value);
            if (this.Type == IndexDataType.UInt64) return ((UInt64)Value).CompareTo(other.Value);
            // decimal
            if (this.Type == IndexDataType.Single) return ((Single)Value).CompareTo(other.Value);
            if (this.Type == IndexDataType.Double) return ((Double)Value).CompareTo(other.Value);
            if (this.Type == IndexDataType.Decimal) return ((Decimal)Value).CompareTo(other.Value);
            // string
            if (this.Type == IndexDataType.String) return string.Compare((String)Value, other.Value.ToString(), StringComparison.InvariantCultureIgnoreCase);

            // other
            if (this.Type == IndexDataType.DateTime) return ((DateTime)Value).CompareTo(other.Value);
            if (this.Type == IndexDataType.Guid) return ((Guid)Value).CompareTo(other.Value);

 	        throw new NotImplementedException();
        }

        public override string ToString()
        {
            return Value == null ? "(null)" : Value.ToString();
        }
    }
}
