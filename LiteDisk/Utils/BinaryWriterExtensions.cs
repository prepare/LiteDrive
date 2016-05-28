//MIT, 2014-2015 Mauricio David
using System;
using System.Collections.Generic;

using System.Text;
using System.IO;
using System.Threading;

namespace LiteDB
{
    static class BinaryWriterExtensions
    {
        public static void Write(this BinaryWriter writer, string text, int length)
        {
            if (string.IsNullOrEmpty(text))
            {
                writer.Write(new byte[length]);
                return;
            }

            var buffer = new byte[length];
            var strbytes = Encoding.UTF8.GetBytes(text);

            Array.Copy(strbytes, buffer, length > strbytes.Length ? strbytes.Length : length);

            writer.Write(buffer);
        }

        public static void Write(this BinaryWriter writer, Guid guid)
        {
            writer.Write(guid.ToByteArray());
        }

        public static void Write(this BinaryWriter writer, DateTime dateTime)
        {
            writer.Write(dateTime.Ticks);
        }

        public static void Write(this BinaryWriter writer, PageAddress address)
        {
            writer.Write(address.PageID);
            writer.Write(address.Index);
        }

        public static void Write(this BinaryWriter writer, IndexKey obj)
        {
            writer.Write((byte)obj.Type);

            switch (obj.Type)
            {
                // int
                case IndexDataType.Byte: writer.Write((Byte)obj.Value); break;
                case IndexDataType.Int16: writer.Write((Int16)obj.Value); break;
                case IndexDataType.UInt16: writer.Write((UInt16)obj.Value); break;
                case IndexDataType.Int32: writer.Write((int)obj.Value); break;
                case IndexDataType.UInt32: writer.Write((UInt32)obj.Value); break;
                case IndexDataType.Int64: writer.Write((Int64)obj.Value); break;
                case IndexDataType.UInt64: writer.Write((UInt64)obj.Value); break;
                //floating number
                case IndexDataType.Single: writer.Write((Single)obj.Value); break;
                case IndexDataType.Double: writer.Write((Double)obj.Value); break;
                case IndexDataType.Decimal: writer.Write((Decimal)obj.Value); break;
                //string
                case IndexDataType.String:
                    {
                        //TODO: review here
                        int length = Encoding.UTF8.GetByteCount((String)obj.Value);
                        if (length > byte.MaxValue)
                        {
                            throw new NotSupportedException("string index not more than " + byte.MaxValue);
                        }
                        writer.Write((byte)length);
                        writer.Write((String)obj.Value, length);
                    } break;
                case IndexDataType.DateTime:
                    writer.Write((DateTime)obj.Value);
                    break;
                case IndexDataType.Guid:
                    writer.Write((Guid)obj.Value);
                    break;
                case IndexDataType.Null:
                    break;
                default:
                    throw new NotSupportedException("unknown type");
            }

        }

        public static long Seek(this BinaryWriter writer, long position)
        {
            return writer.BaseStream.Seek(position, SeekOrigin.Begin);
        }
    }
}
