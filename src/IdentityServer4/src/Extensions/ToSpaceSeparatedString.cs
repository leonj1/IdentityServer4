using System;
using System.Collections.Generic;
using System.Text;

namespace IdentityServer4.Extensions
{
    public static class ToSpaceSeparatedStringExtensions
    {
        [DebuggerStepThrough]
        public static string ToSpaceSeparatedString(this IEnumerable<string> list)
        {
            if (list == null)
                return null;

            var sb = new StringBuilder();
            foreach (var item in list)
            {
                if (sb.Length > 0)
                    sb.Append(" ");
                sb.Append(item);
            }
            return sb.ToString();
        }
    }
}
