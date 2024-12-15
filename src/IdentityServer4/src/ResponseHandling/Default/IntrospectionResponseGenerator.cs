// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityModel;
using IdentityServer4.Events;
using IdentityServer4.Extensions;
using IdentityServer4.Services;
using IdentityServer4.Validation;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServer4.ResponseHandling
{
    /// <summary>
    /// The introspection response generator
    /// </summary>
    /// <seealso cref="IdentityServer4.ResponseHandling.IIntrospectionResponseGenerator" />
    public class IntrospectionResponseGenerator : IIntrospectionResponseGenerator
    {
        /// <summary>
        /// Gets the events.
        /// </summary>
        /// <value>
        /// The events.
        /// </value>
        protected readonly IEventService Events;

        /// <summary>
        /// The logger
        /// </summary>
        protected readonly ILogger Logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="IntrospectionResponseGenerator" /> class.
        /// </summary>
        /// <param name="events">The events.</param>
        /// <param name="logger">The logger.</param>
        public IntrospectionResponseGenerator(IEventService events, ILogger<IntrospectionResponseGenerator> logger)
        {
            Events = events;
            Logger = logger;
        }

        /// <summary>
        /// Processes the response.
        /// </summary>
        /// <param name="validationResult">The validation result.</param>
        /// <returns></returns>
        public virtual async Task<Dictionary<string, object>> ProcessAsync(IntrospectionRequestValidationResult validationResult)
        {
            Logger.LogTrace("Creating introspection response");

            // standard response
            var response = new Dictionary<string, object>
            {
                { "active", false }
            };

            // token is invalid
            if (validationResult.IsActive == false)
            {
                Logger.LogDebug("Creating introspection response for inactive token.");
                await Events.RaiseAsync(new TokenIntrospectionSuccessEvent(validationResult));

                return response;
            }

            // Check if the API resource is allowed to introspect the scopes.
            if (!await AreExpectedScopesPresentAsync(validationResult))
            {
                return response;
            }

            // At least one of the scopes the API supports is in the token
            var tokenScopesThatMatchApi = validationResult.Claims.Where(c => c.Type == JwtClaimTypes.Scope).Where(c => validationResult.Api.Scopes.Contains(c.Value));

            if (tokenScopesThatMatchApi.Any())
            {
                response.Add("scope", string.Join(" ", tokenScopesThatMatchApi.Select(s => s.Value)));
            }

            await Events.RaiseAsync(new TokenIntrospectionSuccessEvent(validationResult));

            return response;
        }

        /// <summary>
        /// Checks if the API resource is allowed to introspect the scopes.
        /// </summary>
        /// <param name="validationResult">The validation result.</param>
        /// <returns></returns>
        protected virtual async Task<bool> AreExpectedScopesPresentAsync(IntrospectionRequestValidationResult validationResult)
        {
            var tokenScopesThatMatchApi = validationResult.Claims.Where(c => c.Type == JwtClaimTypes.Scope).Where(c => validationResult.Api.Scopes.Contains(c.Value));

            if (tokenScopesThatMatchApi.Any())
            {
                // at least one of the scopes the API supports is in the token
                return true;
            }

            // no scopes for this API are found in the token
            Logger.LogError("Expected scope {scopes} is missing in token", validationResult.Api.Scopes);
            await Events.RaiseAsync(new TokenIntrospectionFailureEvent(validationResult.Api.Name, "Expected scopes are missing", validationResult.Token, validationResult.Api.Scopes, tokenScopesThatMatchApi.Select(s => s.Value)));

            return false;
        }
    }
}
