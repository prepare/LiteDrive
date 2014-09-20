using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace LiteDB
{
    public partial class Collection
    {
        /// <summary>
        /// Insert a object on collection using a key
        /// </summary>
        public virtual void Insert(BsonDocument doc)
        {
            if (doc == null) throw new ArgumentNullException("doc");
            if (doc.Id == null) throw new ArgumentNullException("doc.Id");

            // serialize object
            var bytes = doc.ToBson();

            _engine.Transaction.Begin();

            try
            {
                var col = this.GetCollectionPage();

                // storage in data pages - returns dataBlock address
                var dataBlock = _engine.Data.Insert(col, bytes);

                // store id in a PK index [0 array]
                var pk = _engine.Indexer.AddNode(col.PK, doc.Id);

                // do links between index <-> data block
                pk.DataBlock = dataBlock.Position;
                dataBlock.IndexRef[0] = pk.Position;

                // for each index, insert new IndexNode
                for (byte i = 1; i < col.Indexes.Length; i++)
                {
                    var index = col.Indexes[i];

                    if (!index.IsEmpty)
                    {
                        var key = doc.GetFieldValue(index.Field);

                        var node = _engine.Indexer.AddNode(index, key);

                        // point my index to data object
                        node.DataBlock = dataBlock.Position;

                        // point my dataBlock
                        dataBlock.IndexRef[i] = node.Position;
                    }
                }

                _engine.Transaction.Commit();
            }
            catch (Exception ex)
            {
                _engine.Transaction.Rollback();
                throw ex;
            }
        }
    }
}
