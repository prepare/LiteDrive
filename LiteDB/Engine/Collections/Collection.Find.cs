using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace LiteDB
{
    public partial class Collection<T>
    {
        public T FindById(object id)
        {
            var col = this.GetCollectionPage();

            var node = _engine.Indexer.FindOne(col.PK, id);

            if (node == null) return default(T);

            var dataBlock = _engine.Data.Read(node.DataBlock, true);

            return BsonSerializer.Deserialize<T>(dataBlock.Key, dataBlock.Data);
        }

        public T FindOne(Query query)
        {
            return this.Find(query).FirstOrDefault();
        }

        /// <summary>
        /// Find objects inside a collection using a index. Index must exists
        /// </summary>
        public IEnumerable<T> Find(Query query)
        {
            var col = this.GetCollectionPage();

            var nodes = query.Run(_engine, col);

            foreach (var node in nodes)
            {
                var dataBlock = _engine.Data.Read(node.DataBlock, true);

                yield return BsonSerializer.Deserialize<T>(dataBlock.Key, dataBlock.Data);
            }
        }

        /// <summary>
        /// Get object count using property on collection.
        /// </summary>
        /// <returns></returns>
        public int Count()
        {
            var col = this.GetCollectionPage();

            return Convert.ToInt32(col.DocumentCount);
        }

        /// <summary>
        /// Count objects with a query. Do not read objects
        /// </summary>
        public int Count(Query query)
        {
            var col = this.GetCollectionPage();

            return query.Run(_engine, col).Count();
        }

        /// <summary>
        /// Returns true if query returns any object. Do not read objects
        /// </summary>
        public bool Exists(Query query)
        {
            var col = this.GetCollectionPage();

            return query.Run(_engine, col).FirstOrDefault() != null;
        }
    }
}
