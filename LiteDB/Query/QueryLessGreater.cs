using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace LiteDB
{
    internal class QueryLessGreater : Query
    {
        public object Value { get; private set; }
        public bool Include { get; private set; }
        public bool IsLess { get; private set; }

        public QueryLessGreater(string field, object value, bool include, bool isLess)
            : base(field)
        {
            this.Value = value;
            this.Include = include;
            this.IsLess = isLess;
        }

        internal override IEnumerable<IndexNode> Execute(LiteEngine engine, CollectionIndex index)
        {
            if (this.IsLess)
            {
                return engine.Indexer.FindLessThan(index, this.Value, this.Include);
            }
            else
            {
                return engine.Indexer.FindGreaterThan(index, this.Value, this.Include);
            }
        }
    }
}
