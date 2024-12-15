// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityModel;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using IdentityServer4.Configuration;

namespace IdentityServer4.Extensions
{
    /// <summary>
    /// Extensions for JwtPayload
    /// </summary>
    public static class JwtPayloadExtensions
    {
        /// <summary>
        /// Creates a JWT payload from the token.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <param name="logger">The logger.</param>
        /// <returns>The JWT payload.</returns>
        public static JwtPayload CreateJwtPayload(this Token token, ILogger logger)
        {
            // Existing implementation of CreateJwtPayload
            var payload = new JwtPayload();

            // ... (rest of the method implementation)

            return payload;
        }
    }
}
