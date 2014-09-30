using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace LiteDB
{
    /// <summary>
    /// Storage is a special collection to store files/streams.
    /// </summary>
    public partial class Storage
    {
        private Collection<BsonDocument> _col;
        private LiteEngine _engine;

        internal Storage(LiteEngine engine)
        {
            _engine = engine;
            _col = _engine.GetCollection("_files");
        }
    }
}
