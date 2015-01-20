using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.Collections.Specialized;

namespace LiteDB
{
    internal static class NameValueCollectionExtensions
    {
        public static void ParseQueryString(this NameValueCollection nameValueCollection, string queryString)
        {
            nameValueCollection.Clear();

            var querySegments = queryString.Split('&');

            foreach (var segment in querySegments)
            {
                var parts = segment.Split('=');
                if (parts.Length > 0)
                {
                    var key = parts[0].Trim(new char[] { '?', ' ' });
                    var val = parts[1].Trim();

                    nameValueCollection.Add(key, val);
                }
            }
        }
    }
}
