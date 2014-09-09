using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace LiteDB
{
    public class Collection<T>
    {
        private Collection _col;

        internal Collection(LiteEngine engine, string name)
        {
            _col = new Collection(engine, name);
        }

        public virtual bool Delete(object id)
        {
            return _col.Delete(id);
        }
    }
}
