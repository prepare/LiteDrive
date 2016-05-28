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
        /// Delete a document in collection using Document Id - returns false if not found document
        /// </summary>
        public virtual bool Delete(object id)
        {
            var col = this.GetCollectionPage();

            // find indexNode using PK index
            var node = _engine.Indexer.FindOne(col.PK, id);

            if (node == null) return false;

            // start transaction - if clear cache, get again collection page
            if (_engine.Transaction.Begin())
            {
                col = this.GetCollectionPage();
            }

            try
            {
                this.Delete(col, node);

                _engine.Transaction.Commit();

                return true;
            }
            catch (Exception ex)
            {
                _engine.Transaction.Rollback();
                throw ex;
            }
        }

        void Delete(CollectionPage col, IndexNode node)
        {
            // read dataBlock 
            var dataBlock = _engine.Data.Read(node.DataBlock, false);

            // lets remove all indexes that point to this in dataBlock
            for (byte i = 0; i < col.Indexes.Length; i++)
            {
                var index = col.Indexes[i];

                if (!index.IsEmpty)
                {
                    _engine.Indexer.Delete(index, dataBlock.IndexRef[i]);
                }
            }

            // remove object data
            _engine.Data.Delete(col, node.DataBlock);
        }
    }
}
