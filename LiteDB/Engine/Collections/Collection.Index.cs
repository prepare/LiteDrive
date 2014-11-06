using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace LiteDB
{
    public partial class Collection<T>
    {
        /// <summary>
        /// Create a new permanent index in all documents inside this collections if index not exists already.
        /// </summary>
        /// <param name="field">Document field name (case sensitive)</param>
        /// <param name="unique">Create a unique values index?</param>
        public virtual void EnsureIndex(string field, bool unique = false)
        {
            if (string.IsNullOrEmpty(field)) throw new ArgumentNullException("field");

            var col = this.GetCollectionPage();

            // first, check if index already exists
            if (col.Indexes.FirstOrDefault(x => x.Field == field) != null) return;

            // start transaction - if clear cache, get again collection page
            if (_engine.Transaction.Begin())
            {
                col = this.GetCollectionPage();
            }

            try
            {
                // get index slot
                var slot = col.GetFreeIndex();

                // create index head
                var index = _engine.Indexer.CreateIndex(col.Indexes[slot]);

                index.Field = field;
                index.Unique = unique;

                // read all objects (read from PK index)
                foreach (var node in _engine.Indexer.FindAll(col.PK))
                {
                    var dataBlock = _engine.Data.Read(node.DataBlock, true);

                    // read object
                    var doc = BsonSerializer.Deserialize<T>(dataBlock.Key, dataBlock.Data);

                    // adding index
                    var key = BsonSerializer.GetFieldValue(doc, field);

                    var newNode = _engine.Indexer.AddNode(index, key);

                    // adding this new index Node to indexRef
                    dataBlock.IndexRef[slot] = newNode.Position;

                    // link index node to datablock
                    newNode.DataBlock = dataBlock.Position;

                    // mark datablock page as dirty
                    dataBlock.Page.IsDirty = true;
                }

                _engine.Transaction.Commit();
            }
            catch (Exception ex)
            {
                _engine.Transaction.Rollback();
                throw ex;
            }
        }

        /// <summary>
        /// Drop index and release slot for another index
        /// </summary>
        public void DropIndex(string field)
        {
            throw new NotImplementedException();
        }
    }
}
