using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace LiteDB
{
    internal class QueryStartsWith : Query
    {
        public string Value { get; private set; }
        public StringComparison StringComparison { get; private set; }

        public QueryStartsWith(string field, string value, StringComparison stringComparison = StringComparison.Ordinal)
            : base(field)
        {
            this.Value = value;
            this.StringComparison = StringComparison;
        }

        internal override IEnumerable<IndexNode> Execute(LiteEngine engine, CollectionIndex index)
        {
            return engine.Indexer.FindStarstWith(index, this.Value, this.StringComparison);
        }
    }
}
