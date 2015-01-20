﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace LiteDB
{
    /// <summary>
    /// Represent a document schemeless to use in collections.
    /// </summary>
    public class BsonDocument : BsonObject
    {
        public const int MAX_DOCUMENT_SIZE = 256 * 1024; // limits in 256 max document size to avoid large documents, memory usage and slow performance

        public BsonDocument()
            : base()
        {
        }

        public object Id { get; set; } 

        internal BsonDocument(Dictionary<string, object> obj)
            : base(obj)
        {
        }
    }
}
