using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace LiteDB
{
    public partial class Collection<T>
    {
        /// <summary>
        /// Find a document using Document Id. Returns null if not found.
        /// </summary>
        public T FindById(object id)
        {
            if (id == null) throw new ArgumentNullException("id");

            var col = this.GetCollectionPage();

            var node = _engine.Indexer.FindOne(col.PK, id);

            if (node == null) return default(T);

            var dataBlock = _engine.Data.Read(node.DataBlock, true);

            return BsonSerializer.Deserialize<T>(dataBlock.Key, dataBlock.Data);
        }

        /// <summary>
        /// Find the first document using Query object. Returns null if not found.
        /// </summary>
        public T FindOne(Query query)
        {
            return this.Find(query).FirstOrDefault();
        }

        /// <summary>
        /// Find documents inside a collection using Query object.
        /// </summary>
        public IEnumerable<T> Find(Query query)
        {
            if (query == null) throw new ArgumentNullException("query");

            var col = this.GetCollectionPage();

            var nodes = query.Run(_engine, col);

            foreach (var node in nodes)
            {
                var dataBlock = _engine.Data.Read(node.DataBlock, true);

                yield return BsonSerializer.Deserialize<T>(dataBlock.Key, dataBlock.Data);
            }
        }

        /// <summary>
        /// Get document count using property on collection.
        /// </summary>
        public int Count()
        {
            var col = this.GetCollectionPage();

            return Convert.ToInt32(col.DocumentCount);
        }

        /// <summary>
        /// Count documnets with a query. This method does not deserialize any document.
        /// </summary>
        public int Count(Query query)
        {
            if (query == null) throw new ArgumentNullException("query");

            var col = this.GetCollectionPage();

            return query.Run(_engine, col).Count();
        }

        /// <summary>
        /// Returns true if query returns any document. This method does not deserialize any document.
        /// </summary>
        public bool Exists(Query query)
        {
            if (query == null) throw new ArgumentNullException("query");

            var col = this.GetCollectionPage();

            return query.Run(_engine, col).FirstOrDefault() != null;
        }
    }
}
