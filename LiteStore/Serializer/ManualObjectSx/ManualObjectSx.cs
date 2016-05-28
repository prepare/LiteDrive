﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace LiteDB
{
    public delegate void SerializePlanBuilder<T>(TypeStringKeyRWPlan<T> writer);
    public delegate K GetId<T, K>(T obj);
    public delegate void OnReadField<T, V>(T obj, V value);
    public delegate V OnWriteField<T, V>(T obj);

    public static class ManualObjSx<T>
    {
        public static ManualObjectSx<T, K> Build<K>(
            GetId<T, K> getIdMethod,
            SerializePlanBuilder<T> serializerMethod)
        {
            var sx1 = new ManualObjectSx<T, K>();
            sx1.SetSerializeMethod(getIdMethod, serializerMethod);
            return sx1;
        }
    }

    public static class ManualObjSx
    {
        public static ManualObjectSx<T, K> Build<T, K>(
            IEnumerable<T> sample,
            GetId<T, K> getIdMethod,
            SerializePlanBuilder<T> serializerMethod)
        {
            var sx1 = new ManualObjectSx<T, K>();
            sx1.SetSerializeMethod(getIdMethod, serializerMethod);
            return sx1;
        }
        public static ManualObjectSx<T, K> Build<T, K>(
         T sample,
         GetId<T, K> getIdMethod,
         SerializePlanBuilder<T> serializerMethod)
        {
            var sx1 = new ManualObjectSx<T, K>();
            sx1.SetSerializeMethod(getIdMethod, serializerMethod);
            return sx1;
        }
    }
    /// <summary>
    /// manaual object serializer/ deserializer
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="K"></typeparam>
    public class ManualObjectSx<T, K> : ObjectSerializer, IDisposable
    {
        MemoryStream ms;
        BinaryWriter binWriter;
        BinaryReader binReader;

        byte[] buffer;
        K objectId;
        T currentObject;
        TypeStringKeyRWPlan<T> typeRW;
        SerializePlanBuilder<T> serializePlanBuild;
        GetId<T, K> getIdMethod;


        public ManualObjectSx()
        {
            this.ms = new MemoryStream();
            this.binWriter = new BinaryWriter(ms);
            this.binReader = new BinaryReader(ms);
            this.typeRW = new TypeStringKeyRWPlan<T>(binReader, binWriter);
        }
        public void Dispose()
        {
            if (binWriter != null)
            {
                binWriter.Close();
                binWriter = null;
            }
            if (binReader != null)
            {
                binReader.Close();
            }
            if (ms != null)
            {
                ms.Close();
                ms.Dispose();
                ms = null;
            }
        }
        public override object Id
        {
            get
            {
                return this.objectId;
            }
        }
        public void SetSerializeMethod(
            GetId<T, K> getIdMethod,
            SerializePlanBuilder<T> serializerMethod)
        {
            this.getIdMethod = getIdMethod;
            this.serializePlanBuild = serializerMethod;
            serializePlanBuild(typeRW);

        }
        public void Load<U>(int objectId, U obj)
            where U : class,T
        {
            //T is a single object 
            this.objectId = getIdMethod(obj);
            ToBlob(obj);
        }
        public override object GetFieldValue(string fieldName)
        {
            throw new NotImplementedException();
        }
        public override byte[] GetBlob()
        {
            return this.buffer;
        }
        public T FromBlob<U>(byte[] blobData)
            where U : T, new()
        {

            U u = new U();
            ConvertFromBlob(u, blobData);
            return u;
            //-----------------------
        }
        public T ConvertFromBlob(T instance, byte[] blobData)
        {
            ms.Position = 0;
            ms.Write(blobData, 0, blobData.Length);
            ms.Position = 0;
            //-----------------------             
            //read data and set
            typeRW.DoReadPlan(instance);
            return instance;
            //-----------------------
        }
        public byte[] ToBlob<U>(U obj)
            where U : class,T
        {
            if (obj == null)
            {
                return new byte[0];
            }
            //------------------------------------------------
            this.currentObject = obj;

            ms.Position = 0;//move to start position

            typeRW.DoWritePlan(obj);

            int len = (int)ms.Position;
            binWriter.Flush();
            ms.Position = 0;

            byte[] buffer = new byte[len];
            ms.Read(buffer, 0, len);
            return this.buffer = buffer;
        }
    }


    public abstract class TypeReadWritePlanBase<U>
    {
    }
    /// <summary>
    /// type readwrite plan
    /// </summary>
    /// <typeparam name="K"></typeparam>
    /// <typeparam name="T"></typeparam>
    public abstract class TypeReadWritePlan<K, T> : TypeReadWritePlanBase<T>
    {
        //K= key type
        //T= data
        protected readonly BinaryReader reader;
        protected readonly BinaryWriter writer;
        protected readonly List<Action<T>> writeList = new List<Action<T>>();
        protected readonly Dictionary<K, Action<T>> readActions = new Dictionary<K, Action<T>>();

        public TypeReadWritePlan(BinaryReader reader, BinaryWriter writer)
        {
            this.reader = reader;
            this.writer = writer;
        }
        public abstract void RW(K onkey, OnReadField<T, int> onReadField, OnWriteField<T, int> onWriteField);
        public abstract void RW(K onkey, OnReadField<T, string> onReadField, OnWriteField<T, string> onWriteField);
        public abstract void RW(K onkey, OnReadField<T, byte[]> onReadField, OnWriteField<T, byte[]> onWriteField);



        //--------------------------------------------------------------
        public void R(K onkey, OnReadField<T, int> onReadField)
        {
            RW(onkey, onReadField, null);
        }
        public void R(K onkey, OnReadField<T, string> onReadField)
        {
            RW(onkey, onReadField, null);
        }
        public void R(K onkey, OnReadField<T, byte[]> onReadField)
        {
            RW(onkey, onReadField, null);
        }
        public void W(K onkey, OnWriteField<T, int> onWriteField)
        {
            RW(onkey, null, onWriteField);
        }
        public void W(K onkey, OnWriteField<T, string> onWriteField)
        {
            RW(onkey, null, onWriteField);
        }
        public void W(K onkey, OnWriteField<T, byte[]> onWriteField)
        {
            RW(onkey, null, onWriteField);
        }
        //--------------------------------------------------------------

        public abstract K ReadKey();
        public virtual void DoWritePlan(T obj)
        {
            int j = writeList.Count;
            writer.Write((ushort)j); //num of field to write
            for (int i = 0; i < j; ++i)
            {
                writeList[i](obj);
            }
        }
        public virtual void DoReadPlan(T obj)
        {
            //num of field
            int j = reader.ReadUInt16();
            //read key
            for (int i = 0; i < j; ++i)
            {
                K key = ReadKey();
                //read value
                Action<T> foundAction;
                if (readActions.TryGetValue(key, out  foundAction))
                {
                    foundAction(obj);
                }
                else
                {
                    throw new NotSupportedException("unknown field!");
                }
            }
        }

    }


    public class TypeStringKeyRWPlan<T> : TypeReadWritePlan<string, T>
    {

        internal TypeStringKeyRWPlan(BinaryReader reader, BinaryWriter writer)
            : base(reader, writer)
        {
        }
        public override string ReadKey()
        {
            return reader.ReadString();
        }
        public override void RW(string onkey, OnReadField<T, int> onReadField, OnWriteField<T, int> onWriteField)
        {
            if (onWriteField != null)
            {
                writeList.Add(new Action<T>(t =>
                {
                    writer.Write(onkey);
                    writer.Write((byte)BsonType.Int32);
                    writer.Write(onWriteField(t));
                }));
            }

            if (onReadField != null)
            {
                readActions.Add(onkey, new Action<T>(t =>
                {
                    var fieldType = (BsonType)reader.ReadByte();
                    if (fieldType != BsonType.Int32) throw new NotSupportedException();
                    onReadField(t, reader.ReadInt32());
                }));
            }


        }
        public override void RW(string onkey, OnReadField<T, string> onReadField, OnWriteField<T, string> onWriteField)
        {
            if (onWriteField != null)
            {
                writeList.Add(new Action<T>(t =>
                {
                    writer.Write(onkey);
                    string str = onWriteField(t);
                    if (str == null)
                    {
                        writer.Write((byte)BsonType.Null);
                    }
                    else
                    {
                        writer.Write((byte)BsonType.String);
                        writer.Write(onWriteField(t));
                    }
                }));

            }
            if (onReadField != null)
            {
                readActions.Add(onkey, new Action<T>(t =>
                {
                    var fieldType = (BsonType)reader.ReadByte();
                    switch (fieldType)
                    {
                        case BsonType.Null:
                            //not set this field
                            break;
                        case BsonType.String:
                            onReadField(t, reader.ReadString());
                            break;
                        default:
                            throw new NotSupportedException();
                    }

                }));
            }


        }
        public override void RW(string onkey, OnReadField<T, byte[]> onReadField, OnWriteField<T, byte[]> onWriteField)
        {

            if (onWriteField != null)
            {
                writeList.Add(new Action<T>(t =>
                {
                    writer.Write(onkey);
                    byte[] data = onWriteField(t);
                    if (data == null)
                    {
                        writer.Write((byte)BsonType.Null);
                    }
                    else
                    {
                        writer.Write((byte)BsonType.Binary);                         
                        writer.Write(data.Length); //blob len
                        writer.Write(data); //actual blob data
                    }
                }));
            }

            if (onReadField != null)
            {
                readActions.Add(onkey, new Action<T>(t =>
                {
                    var fieldType = (BsonType)reader.ReadByte();
                    switch (fieldType)
                    {
                        case BsonType.Null:
                            //not set this field
                            break;
                        case BsonType.Binary:
                            int blobLen = reader.ReadInt32();
                            byte[] blobData = reader.ReadBytes(blobLen);
                            onReadField(t, blobData);
                            break;
                        default:
                            throw new NotSupportedException();
                    }

                }));
            }

        }

    }
}