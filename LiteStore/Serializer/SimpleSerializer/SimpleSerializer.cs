using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace LiteDB
{
    public delegate void SerializePlanBuilder<T>(TypeReadWritePlan<T> writer);
    public delegate K GetId<T, K>(T obj);
    public delegate void OnSetField<T, V>(T obj, V value);
    public delegate V GetValue<T, V>(T obj);

    public static class MySimpleObjectSx<T>
    {
        public static MySimpleObjectSerializer<T, K> Build<K>(
            GetId<T, K> getIdMethod,
            SerializePlanBuilder<T> serializerMethod)
        {
            var sx1 = new MySimpleObjectSerializer<T, K>();
            sx1.SetSerializeMethod(getIdMethod, serializerMethod);
            return sx1;
        }
    }
    public class MySimpleObjectSerializer<T, K> : ObjectSerializer, IDisposable
    {
        MemoryStream ms;
        BinaryWriter binWriter;
        BinaryReader binReader;

        byte[] buffer;
        K objectId;
        T currentObject;
        TypeReadWritePlan<T> typeRW;
        SerializePlanBuilder<T> serializePlanBuild;
        GetId<T, K> getIdMethod;


        public MySimpleObjectSerializer()
        {
            this.ms = new MemoryStream();
            this.binWriter = new BinaryWriter(ms);
            this.binReader = new BinaryReader(ms);
            this.typeRW = new TypeReadWritePlan<T>(binReader, binWriter);
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
        public void Load(int objectId, T obj)
        {
            //T is a single object
            this.currentObject = obj;
            this.objectId = getIdMethod(obj);
            ms.Position = 0;//move to start position

            typeRW.DoWritePlan(obj);

            int len = (int)ms.Position;
            binWriter.Flush();
            ms.Position = 0;

            byte[] buffer = new byte[len];
            ms.Read(buffer, 0, len);
            this.buffer = buffer;
        }
        public override object GetFieldValue(string fieldName)
        {
            throw new NotImplementedException();
        }
        public override byte[] GetBlob()
        {
            return this.buffer;
        }
        public T ConvertFromBlob<U>(byte[] blobData)
            where U : T, new()
        {
            ms.Position = 0;
            ms.Write(blobData, 0, blobData.Length);
            ms.Position = 0;
            //-----------------------
            U u = new U();
            //read data and set
            typeRW.DoReadPlan(u);
            return u;
            //-----------------------
        }
    }


    public abstract class TypeReadWritePlanBase<K, T>
    {
        //K= key type
        //T= data
        protected readonly BinaryReader reader;
        protected readonly BinaryWriter writer;
        protected readonly List<Action<T>> writeList = new List<Action<T>>();
        protected readonly Dictionary<K, Action<T>> readActions = new Dictionary<K, Action<T>>();

        public TypeReadWritePlanBase(BinaryReader reader, BinaryWriter writer)
        {
            this.reader = reader;
            this.writer = writer;
        }
        public abstract void Set(K onkey, GetValue<T, int> getField, OnSetField<T, int> setField);
        public abstract void Set(K onkey, GetValue<T, string> getField, OnSetField<T, string> setField);
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

    public class TypeReadWritePlan<T> : TypeReadWritePlanBase<string, T>
    {

        internal TypeReadWritePlan(BinaryReader reader, BinaryWriter writer)
            : base(reader, writer)
        {
        }
        public override string ReadKey()
        {
            return reader.ReadString();
        }
        public override void Set(string onkey, GetValue<T, int> getField, OnSetField<T, int> setField)
        {
            writeList.Add(new Action<T>(t =>
            {
                writer.Write(onkey);
                writer.Write((byte)BsonType.Int32);
                writer.Write(getField(t));
            }));

            readActions.Add(onkey, new Action<T>(t =>
            {
                var fieldType = (BsonType)reader.ReadByte();
                if (fieldType != BsonType.Int32) throw new NotSupportedException();
                setField(t, reader.ReadInt32());
            }));
        }
        public override void Set(string onkey, GetValue<T, string> getField, OnSetField<T, string> setField)
        {
            writeList.Add(new Action<T>(t =>
            {
                writer.Write(onkey);
                writer.Write((byte)BsonType.String);
                writer.Write(getField(t));
            }));

            readActions.Add(onkey, new Action<T>(t =>
            {
                var fieldType = (BsonType)reader.ReadByte();
                if (fieldType != BsonType.String) throw new NotSupportedException();
                setField(t, reader.ReadString());
            }));
        }


    }
}