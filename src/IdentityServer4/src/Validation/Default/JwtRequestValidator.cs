// Copyright (c) Brock Allen & Dominick Baier & Microsoft Corporation & contributors.
// Licensed under the Apache License, Version 2.0.

using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServer4.Validation
{
    public class JwtRequestValidator
    {
        private readonly IClientStore _clientStore;

        public JwtRequestValidator(IClientStore clientStore)
        {
            _clientStore = clientStore;
        }

        public async Task<JwtRequestValidationResult> ValidateAsync(string jwtTokenString, Client client)
        {
            var trustedKeys = await GetKeysAsync(client);
            if (!trustedKeys.Any())
            {
                return new JwtRequestValidationResult
                {
                    IsError = true,
                    Error = "There are no keys available to validate JWT."
                };
            }

            try
            {
                var jwtSecurityToken = await ValidateJwtAsync(jwtTokenString, trustedKeys, client);
                if (jwtSecurityToken.Payload.ContainsKey(OidcConstants.AuthorizeRequest.Request) ||
                    jwtSecurityToken.Payload.ContainsKey(OidcConstants.AuthorizeRequest.RequestUri))
                {
                    return new JwtRequestValidationResult
                    {
                        IsError = true,
                        Error = "JWT payload must not contain request or request_uri"
                    };
                }

                var payload = await ProcessPayloadAsync(jwtSecurityToken);

                return new JwtRequestValidationResult
                {
                    IsError = false,
                    Payload = payload
                };
            }
            catch (Exception e)
            {
                return new JwtRequestValidationResult
                {
                    IsError = true,
                    Error = "JWT token validation error",
                    ErrorDetails = e.Message
                };
            }
        }

        protected virtual async Task<List<SecurityKey>> GetKeysAsync(Client client)
        {
            return await client.ClientSecrets.GetKeysAsync();
        }

        protected virtual async Task<JwtSecurityToken> ValidateJwtAsync(string jwtTokenString, IEnumerable<SecurityKey> keys, Client client)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                IssuerSigningKeys = keys,
                ValidateIssuerSigningKey = true,

                ValidIssuer = client.ClientId,
                ValidateIssuer = true,

                ValidAudience = client.AllowedCorsOrigins.FirstOrDefault(),
                ValidateAudience = true,

                RequireSignedTokens = true,
                RequireExpirationTime = true
            };

            Handler.ValidateToken(jwtTokenString, tokenValidationParameters, out var token);
            
            return (JwtSecurityToken)token;
        }

        protected virtual Task<Dictionary<string, string>> ProcessPayloadAsync(JwtSecurityToken token)
        {
            // filter JWT request claim types
            var payload = new Dictionary<string, string>();
            foreach (var key in token.Payload.Keys)
            {
                if (!Constants.Filters.JwtRequestClaimTypesFilter.Contains(key))
                {
                    var value = token.Payload[key];

                    switch (value)
                    {
                        case string s:
                            payload.Add(key, s);
                            break;
                        case JObject jobj:
                            payload.Add(key, jobj.ToString(Formatting.None));
                            break;
                        case JArray jarr:
                            payload.Add(key, jarr.ToString(Formatting.None));
                            break;
                    }
                }
            }

            return Task.FromResult(payload);
        }
    }

    public class JwtRequestValidationResult
    {
        public bool IsError { get; set; }
        public string Error { get; set; }
        public string ErrorDetails { get; set; }
        public Dictionary<string, string> Payload { get; set; }
    }
}
