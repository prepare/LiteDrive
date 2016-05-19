//MIT 2015, WinterDev
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LiteDB
{
    public abstract class ObjectSerializer
    {
        public abstract object Id { get; }
        public abstract byte[] GetBlob();
        public abstract object GetFieldValue(string fieldName);

    }
}