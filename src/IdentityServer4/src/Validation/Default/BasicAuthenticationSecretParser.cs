// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4.Configuration;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using IdentityModel;
using Microsoft.AspNetCore.Http;

namespace IdentityServer4.Validation
{
    /// <summary>
    /// Parses a Basic Authentication header
    /// </summary>
    public class BasicAuthenticationSecretParser : ISecretParser
    {
        private readonly ILogger _logger;
        private readonly IdentityServerOptions _options;

        /// <summary>
        /// Creates the parser with a reference to identity server options
        /// </summary>
        /// <param name="options">IdentityServer options</param>
        /// <param name="logger">The logger</param>
        public BasicAuthenticationSecretParser(IdentityServerOptions options, ILogger<BasicAuthenticationSecretParser> logger)
        {
            _options = options;
            _logger = logger;
        }

        /// <summary>
        /// Returns the authentication method name that this parser implements
        /// </summary>
        /// <value>
        /// The authentication method.
        /// </value>
        public string AuthenticationMethod => OidcConstants.EndpointAuthenticationMethods.BasicAuthentication;

        /// <summary>
        /// Tries to find a secret that can be used for authentication
        /// </summary>
        /// <returns>
        /// A parsed secret
        /// </returns>
        public Task<ParsedSecret> ParseAsync(HttpContext context)
        {
            _logger.LogDebug("Start parsing Basic Authentication secret");

            var notfound = Task.FromResult<ParsedSecret>(null);
            var authorizationHeader = context.Request.Headers["Authorization"].FirstOrDefault();

            if (authorizationHeader.IsMissing())
            {
                return notfound;
            }

            if (!authorizationHeader.StartsWith("Basic ", StringComparison.OrdinalIgnoreCase))
            {
                return notfound;
            }

            var parameter = authorizationHeader.Substring("Basic ".Length);

            string pair;
            try
            {
                pair = Encoding.UTF8.GetString(
                    Convert.FromBase64String(parameter));
            }
            catch (FormatException)
            {
                _logger.LogWarning("Malformed Basic Authentication credential.");
                return notfound;
            }

            if (pair.Length > 0 && pair.Contains(':'))
            {
                var parts = pair.Split(':', 2);
                var clientId = parts[0];
                var clientSecret = parts.Length == 2 ? parts[1] : null;

                _logger.LogDebug("Basic Authentication secret found");

                if (clientId.IsMissing())
                {
                    _logger.LogWarning("Invalid Basic Authentication format.");
                    return notfound;
                }

                if (clientSecret != null && clientSecret.Length > _options.InputLengthRestrictions.ClientSecret)
                {
                    _logger.LogError("Client secret exceeds maximum length.");
                    return notfound;
                }

                var parsedSecret = new ParsedSecret
                {
                    Id = Decode(clientId),
                    Credential = clientSecret == null ? null : Decode(clientSecret),
                    Type = IdentityServerConstants.ParsedSecretTypes.SharedSecret
                };

                return Task.FromResult(parsedSecret);
            }

            _logger.LogWarning("Invalid Basic Authentication format.");
            return notfound;
        }

        // RFC6749 says individual values must be application/x-www-form-urlencoded
        // 2.3.1
        private string Decode(string value)
        {
            if (value.IsMissing()) return string.Empty;

            return Uri.UnescapeDataString(value.Replace("+", "%20"));
        }
    }
}
