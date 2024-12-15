using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using IdentityModel;
using IdentityServer.UnitTests.Common;
using IdentityServer4.Models;

namespace IdentityServer.UnitTests.Validation.Setup
{
    internal static class TokenFactory
    {
        public static Token CreateIdentityToken(string clientId, string subjectId)
        {
            var clients = Factory.CreateClientStore();

            var claims = new List<Claim> 
            {
                new Claim("sub", subjectId)
            };

            var token = new Token(OidcConstants.TokenTypes.IdentityToken)
            {
                CreationTime = DateTime.UtcNow,
                Audiences = { clientId },
                ClientId = clientId,
                Issuer = "https://idsvr.com",
                Lifetime = 600,
                Claims = claims
            };

            return token;
        }

        public static Token CreateIdentityTokenLong(string clientId, string subjectId, int count)
        {
            var clients = Factory.CreateClientStore();

            var claims = new List<Claim>
            {
                new Claim("sub", subjectId)
            };

            for (int i = 0; i < count; i++)
            {
                claims.Add(new Claim("junk", "x".Repeat(100)));
            }

            var token = new Token(OidcConstants.TokenTypes.IdentityToken)
            {
                CreationTime = DateTime.UtcNow,
                Audiences = { clientId },
                ClientId = clientId,
                Issuer = "https://idsvr.com",
                Lifetime = 600,
                Claims = claims
            };

            return token;
        }
    }
}
