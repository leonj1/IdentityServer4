// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace IdentityServer4.Validation
{
    /// <summary>
    /// Validates a secret based on RS256 signed JWT token
    /// </summary>
    public class PrivateKeyJwtSecretValidator : ISecretValidator
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IReplayCache _replayCache;
        private readonly ILogger _logger;

        private const string Purpose = nameof(PrivateKeyJwtSecretValidator);
        
        /// <summary>
        /// Instantiates an instance of private_key_jwt secret validator
        /// </summary>
        public PrivateKeyJwtSecretValidator(IHttpContextAccessor contextAccessor, IReplayCache replayCache, ILogger<PrivateKeyJwtSecretValidator> logger)
        {
            _contextAccessor = contextAccessor;
            _replayCache = replayCache;
            _logger = logger;
        }

        /// <summary>
        /// Validates a secret
        /// </summary>
        /// <param name="secrets">The stored secrets.</param>
        /// <param name="parsedSecret">The received secret.</param>
        /// <returns>
        /// A validation result
        /// </returns>
        /// <exception cref="System.ArgumentException">ParsedSecret.Credential is null</exception>
        public Task<ValidationResult> ValidateAsync(IEnumerable<Secret> secrets, ParsedSecret parsedSecret)
        {
            if (parsedSecret == null)
                throw new ArgumentNullException(nameof(parsedSecret));

            return ValidateInternal(secrets, parsedSecret);
        }

        private async Task<ValidationResult> ValidateInternal(IEnumerable<Secret> secrets, ParsedSecret parsedSecret)
        {
            if (parsedSecret.Type != SecretType.JwtBearerClientAssertion)
                return ValidationResult.Failed("Invalid secret type.");

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKeyResolver = GetTrustedSigningKeys,
                ValidateAudience = false,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero,
                NameClaimType = "sub",
                RoleClaimType = "role"
            };

            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var principal = await tokenHandler.ValidateTokenAsync(parsedSecret.Credential, tokenValidationParameters);

                var jwtToken = (JwtSecurityToken)principal.Identity as JwtSecurityToken;
                if (jwtToken == null)
                    return ValidationResult.Failed("Invalid JWT token.");

                if (jwtToken.Subject != jwtToken.Issuer)
                {
                    _logger.LogError("Both 'sub' and 'iss' in the client assertion token must have a value of client_id.");
                    return ValidationResult.Failed("Invalid JWT token.");
                }

                var exp = jwtToken.Payload.Exp;
                if (!exp.HasValue)
                {
                    _logger.LogError("exp is missing.");
                    return ValidationResult.Failed("Invalid JWT token.");
                }

                var jti = jwtToken.Payload.Jti;
                if (jti.IsMissing())
                {
                    _logger.LogError("jti is missing.");
                    return ValidationResult.Failed("Invalid JWT token.");
                }

                if (await _replayCache.ExistsAsync(Purpose, jti))
                {
                    _logger.LogError("jti is found in replay cache. Possible replay attack.");
                    return ValidationResult.Failed("Possible replay attack.");
                }
                else
                {
                    await _replayCache.AddAsync(Purpose, jti, DateTimeOffset.FromUnixTimeSeconds(exp.Value).AddMinutes(5));
                }

                return ValidationResult.Success;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "JWT token validation error");
                return ValidationResult.Failed("JWT token validation error.");
            }
        }

        private IEnumerable<SecurityKey> GetTrustedSigningKeys(string tokenType, string kid, out TokenValidationParameters validationParameters)
        {
            // Implement logic to get trusted signing keys
            throw new NotImplementedException();
        }

        private string[] GetValidAudiences()
        {
            var issuerUri = _contextAccessor.HttpContext.GetIdentityServerIssuerUri().EnsureTrailingSlash();
            return new[]
            {
                string.Concat(issuerUri, Constants.ProtocolRoutePaths.Token)
            };
        }
    }
}
