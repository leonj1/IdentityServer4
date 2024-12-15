using System;
using IdentityServer.UnitTests.Common;
using IdentityServer4.Models;

namespace IdentityServer.UnitTests.Validation.Setup
{
    internal static class AccessTokenFactory
    {
        public static Token CreateAccessToken(Client client, string subjectId, int lifetime, params string[] scopes)
        {
            return AccessToken.Create(client, subjectId, lifetime, scopes);
        }

        public static Token CreateAccessTokenLong(Client client, string subjectId, int lifetime, int count, params string[] scopes)
        {
            return AccessToken.CreateLong(client, subjectId, lifetime, count, scopes);
        }
    }
}
