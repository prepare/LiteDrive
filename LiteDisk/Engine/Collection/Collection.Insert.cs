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
        /// Insert a new document to this collection. Document Id must be a new value in collection
        /// </summary>
        public virtual void Insert(ObjectSerializer serializedObject)
        {
            //if (doc == null) throw new ArgumentNullException("doc");

            //// gets document Id
            //var id = BsonSerializer.GetIdValue(doc); 
            if (serializedObject.Id == null) throw new ArgumentNullException("Document Id can't be null");

            // serialize object 
            _engine.Transaction.Begin();

            try
            {
                var col = this.GetCollectionPage();

                // storage in data pages - returns dataBlock address
                var dataBlock = _engine.Data.Insert(col, new IndexKey(serializedObject.Id), 
                    serializedObject.GetBlob());

                // store id in a PK index [0 array]
                var pk = _engine.Indexer.AddNode(col.PK, serializedObject.Id);

                // do links between index <-> data block
                pk.DataBlock = dataBlock.Position;
                dataBlock.IndexRef[0] = pk.Position;

                //---------------------------------------
                // for each index, insert new IndexNode
                for (byte i = 1; i < col.Indexes.Length; i++)
                {
                    var index = col.Indexes[i];

                    if (!index.IsEmpty)
                    {
                        //var key = BsonSerializer.GetFieldValue(doc, index.Field);
                        var key = serializedObject.GetFieldValue(index.Field);
                        

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
