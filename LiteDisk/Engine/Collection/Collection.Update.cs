//MIT, 2014-2015 Mauricio David
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LiteDB
{
    public partial class Collection
    {
        /// <summary>
        /// Update a document in this collection. Returns false if not found document in collection
        /// </summary>
        public virtual bool Update(ObjectSerializer serializedObject)
        {
            if (serializedObject == null) throw new ArgumentNullException("doc");

            // gets document Id
            var id = serializedObject.Id;

            if (id == null) throw new ArgumentNullException("Document Id can't be null");

            var col = this.GetCollectionPage();

            // find indexNode from pk index
            var indexNode = _engine.Indexer.FindOne(col.PK, id);

            if (indexNode == null) return false;

            // serialize object
            var bytes = serializedObject.GetBlob();

            // start transaction - if clear cache, get again collection page
            if (_engine.Transaction.Begin())
            {
                col = this.GetCollectionPage();
            }

            try
            {
                // update data storage
                var dataBlock = _engine.Data.Update(col, indexNode.DataBlock, bytes);
                // delete/insert indexes - do not touch on PK

                for (byte i = 1; i < col.Indexes.Length; i++)
                {
                    var index = col.Indexes[i];

                    if (!index.IsEmpty)
                    {
                        
                        var key = serializedObject.GetFieldValue(index.Field);

                        var node = _engine.Indexer.GetNode(dataBlock.IndexRef[i]);

                        // check if my index node was changed
                        if (node.Key.CompareTo(new IndexKey(key)) != 0)
                        {
                            // remove old index node
                            _engine.Indexer.Delete(index, node.Position);

                            // and add a new one
                            var newNode = _engine.Indexer.AddNode(index, key);

                            // point my index to data object
                            newNode.DataBlock = dataBlock.Position;

                            // point my dataBlock
                            dataBlock.IndexRef[i] = newNode.Position;

                            dataBlock.Page.IsDirty = true;
                        }
                    }
                }

                _engine.Transaction.Commit();

                return true;
            }
            catch (Exception ex)
            {
                _engine.Transaction.Rollback();
                throw ex;
            }
        }
    }
}
