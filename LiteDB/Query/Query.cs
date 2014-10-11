using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace LiteDB
{
    /// <summary>
    /// Class helper to create query using indexes in database. All methods are statics
    /// </summary>
    public abstract class Query
    {
        public string Field { get; private set; }

        internal Query(string field)
        {
            this.Field = field;
        }

        /// <summary>
        /// Returns all objects using _id PK
        /// </summary>
        public static Query All()
        {
            return new QueryAll();
        }

        /// <summary>
        /// Returns all objects using field index order
        /// </summary>
        public static Query All(string field)
        {
            return new QueryAll(field);
        }

        /// <summary>
        /// Returns all objects that value are equals to value (=)
        /// </summary>
        public static Query EQ(string field, object value)
        {
            return new QueryEquals(field, value);
        }

        /// <summary>
        /// Returns all objects that value are less than value (<)
        /// </summary>
        public static Query LT(string field, object value)
        {
            return new QueryLessGreater(field, value, false, true);
        }

        /// <summary>
        /// Returns all objects that value are less than or equals value (<=)
        /// </summary>
        public static Query LTE(string field, object value)
        {
            return new QueryLessGreater(field, value, true, true);
        }

        /// <summary>
        /// Returns all objects that value are greater than value (>)
        /// </summary>
        public static Query GT(string field, object value)
        {
            return new QueryLessGreater(field, value, false, false);
        }

        /// <summary>
        /// Returns all objects that value are greater than or equals value (>=)
        /// </summary>
        public static Query GTE(string field, object value)
        {
            return new QueryLessGreater(field, value, true, false);
        }

        /// <summary>
        /// Returns all objects that values are between "start" and "end" values (BETWEEN)
        /// </summary>
        public static Query Between(string field, object start, object end)
        {
            return new QueryBetween(field, start, end);
        }

        /// <summary>
        /// Returns all objects that starts with value (LIKE)
        /// </summary>
        public static Query StartsWith(string field, string value)
        {
            return new QueryStartsWith(field, value);
        }

        /// <summary>
        /// Returns all objects that starts with value (LIKE)
        /// </summary>
        public static Query StartsWith(string field, string value, StringComparison stringComparison)
        {
            return new QueryStartsWith(field, value, stringComparison);
        }

        /// <summary>
        /// Returns all objects that are not equals to value
        /// </summary>
        public static Query Not(string field, object value)
        {
            return new QueryNot(field, value);
        }

        /// <summary>
        /// Returns all objects that has value in values list (IN)
        /// </summary>
        public static Query In(string field, params object[] values)
        {
            return new QueryIn(field, values);
        }
        /// <summary>
        /// Returns objects that exists in ALL queries results.
        /// </summary>
        public static Query AND(params Query[] queries)
        {
            return new QueryAndOr(queries, true);
        }

        /// <summary>
        /// Returns objects that exists in ANY queries results.
        /// </summary>
        public static Query OR(params Query[] queries)
        {
            return new QueryAndOr(queries, false);
        }

        #region Execute Query

        internal abstract IEnumerable<IndexNode> Execute(LiteEngine engine, CollectionIndex index);

        internal virtual IEnumerable<IndexNode> Run(LiteEngine engine, CollectionPage col)
        {
            var index = col.Indexes.FirstOrDefault(x => x.Field.Equals(this.Field, StringComparison.InvariantCultureIgnoreCase));

            if (index == null) throw new LiteException(string.Format("Index '{0}.{1}' not found. Use EnsureIndex to create a new index.", col.CollectionName, this.Field));

            return this.Execute(engine, index);
        }

        #endregion
    }
}
