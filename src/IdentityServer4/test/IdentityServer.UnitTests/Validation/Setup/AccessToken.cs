using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using IdentityModel;
using IdentityServer.UnitTests.Common;
using IdentityServer4.Models;

namespace IdentityServer.UnitTests.Validation.Setup
{
    public class AccessToken
    {
        public static Token Create(Client client, string subjectId, int lifetime, params string[] scopes)
        {
            var claims = new List<Claim> 
            {
                new Claim("client_id", client.ClientId),
                new Claim("sub", subjectId)
            };

            scopes.ToList().ForEach(s => claims.Add(new Claim("scope", s)));

            var token = new Token(OidcConstants.TokenTypes.AccessToken)
            {
                CreationTime = DateTime.UtcNow,
                Audiences = { "https://idsvr.com/resources" },
                Issuer = "https://idsvr.com",
                Lifetime = lifetime,
                Claims = claims,
                ClientId = client.ClientId,
                AccessTokenType = client.AccessTokenType
            };

            return token;
        }

        public static Token CreateLong(Client client, string subjectId, int lifetime, int count, params string[] scopes)
        {
            var claims = new List<Claim>
            {
                new Claim("client_id", client.ClientId),
                new Claim("sub", subjectId)
            };

            for (int i = 0; i < count; i++)
            {
                claims.Add(new Claim("junk", "x".Repeat(100)));
            }

            scopes.ToList().ForEach(s => claims.Add(new Claim("scope", s)));

            var token = new Token(OidcConstants.TokenTypes.AccessToken)
            {
                CreationTime = DateTime.UtcNow,
                Audiences = { "https://idsvr.com/resources" },
                Issuer = "https://idsvr.com",
                Lifetime = lifetime,
                Claims = claims,
                ClientId = client.ClientId,
                AccessTokenType = client.AccessTokenType
            };

            return token;
        }
    }
}
