using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace LiteDB
{
    internal class QueryAndOr : Query
    {
        public Query[] Queries { get; private set; }
        public bool IsAnd { get; private set; }

        public QueryAndOr(Query[] queries, bool isAnd)
            : base(null)
        {
            this.Queries = queries;
            this.IsAnd = IsAnd;
        }

        // Never runs in AND/OR queries
        internal override IEnumerable<IndexNode> Execute(LiteEngine engine, CollectionIndex index)
        {
            return null;
        }

        internal override IEnumerable<IndexNode> Run(LiteEngine engine, CollectionPage col)
        {
            var results = this.Queries[0].Run(engine, col);

            for (var i = 1; i < this.Queries.Length; i++)
            {
                var q = this.Queries[i];

                if (this.IsAnd)
                {
                    results = results.Intersect(q.Run(engine, col));
                }
                else
                {
                    results = results.Union(q.Run(engine, col));
                }
            }
            return results;
        }
    }
}
