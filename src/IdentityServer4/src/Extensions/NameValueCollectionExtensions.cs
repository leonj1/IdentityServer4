using System;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace IdentityServer4.Extensions
{
    public static class NameValueCollectionExtensions
    {
        public static NameValueCollection FromFullDictionary(this IDictionary<string, string[]> source)
        {
            var nvc = new NameValueCollection();

            foreach ((string key, string[] strings) in source)
            {
                foreach (var value in strings)
                {
                    nvc.Add(key, value);
                }
            }

            return nvc;
        }
    }
}
