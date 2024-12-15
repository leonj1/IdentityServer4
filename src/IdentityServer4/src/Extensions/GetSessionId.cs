using IdentityModel;
using Microsoft.AspNetCore.Authentication;
using System;

namespace IdentityServer4.Extensions
{
    /// <summary>
    /// Extensions for AuthenticationProperties
    /// </summary>
    public static class GetSessionIdExtensions
    {
        internal const string SessionIdKey = "session_id";

        /// <summary>
        /// Gets the user's session identifier.
        /// </summary>
        /// <param name="properties"></param>
        /// <returns></returns>
        public static string GetSessionId(this AuthenticationProperties properties)
        {
            if (properties?.Items.ContainsKey(SessionIdKey) == true)
            {
                return properties.Items[SessionIdKey];
            }

            return null;
        }
    }
}
