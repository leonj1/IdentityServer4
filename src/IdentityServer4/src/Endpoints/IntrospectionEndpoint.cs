// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System.Threading.Tasks;
using IdentityServer4.Validation;
using IdentityServer4.ResponseHandling;
using Microsoft.Extensions.Logging;
using IdentityServer4.Hosting;
using IdentityServer4.Endpoints.Results;
using Microsoft.AspNetCore.Http;
using System.Net;
using IdentityServer4.Services;
using IdentityServer4.Events;
using IdentityServer4.Extensions;

namespace IdentityServer4.Endpoints
{
    /// <summary>
    /// Introspection endpoint
    /// </summary>
    /// <seealso cref="IdentityServer4.Hosting.IEndpointHandler" />
    internal class IntrospectionEndpoint : IEndpointHandler
    {
        private readonly IIntrospectionResponseGenerator _responseGenerator;
        private readonly IEventService _events;
        private readonly ILogger _logger;
        private readonly IIntrospectionRequestValidator _requestValidator;
        private readonly IApiSecretValidator _apiSecretValidator;

        /// <summary>
        /// Initializes a new instance of the <see cref="IntrospectionEndpoint" /> class.
        /// </summary>
        /// <param name="apiSecretValidator">The API secret validator.</param>
        /// <param name="requestValidator">The request validator.</param>
        /// <param name="responseGenerator">The generator.</param>
        /// <param name="events">The events.</param>
        /// <param name="logger">The logger.</param>
        public IntrospectionEndpoint(
            IApiSecretValidator apiSecretValidator,
            IIntrospectionRequestValidator requestValidator,
            IIntrospectionResponseGenerator responseGenerator,
            IEventService events,
            ILogger<IntrospectionEndpoint> logger)
        {
            _apiSecretValidator = apiSecretValidator;
            _requestValidator = requestValidator;
            _responseGenerator = responseGenerator;
            _events = events;
            _logger = logger;
        }

        /// <summary>
        /// Processes the endpoint.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public async Task ProcessAsync(HttpContext context)
        {
            // existing code...
        }

        private void LogSuccess(bool tokenActive, string apiName)
        {
            _logger.LogInformation("Success token introspection. Token active: {tokenActive}, for API name: {apiName}", tokenActive, apiName);
        }

        private void LogFailure(string error, string apiName)
        {
            _logger.LogError("Failed token introspection: {error}, for API name: {apiName}", error, apiName);
        }
    }
}
