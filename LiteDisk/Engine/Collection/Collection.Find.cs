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
        /// Find a document using Document Id. Returns null if not found.
        /// </summary>
        public byte[] FindById(object id)
        {
            if (id == null) throw new ArgumentNullException("id");

            var col = this.GetCollectionPage();

            var node = _engine.Indexer.FindOne(col.PK, id);

            if (node == null) return null;

            var dataBlock = _engine.Data.Read(node.DataBlock, true);
            return dataBlock.Data;
        }

    }
}
