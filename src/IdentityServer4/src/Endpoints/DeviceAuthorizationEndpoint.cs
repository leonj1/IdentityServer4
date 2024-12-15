// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.Endpoints.Results;
using IdentityServer4.Events;
using IdentityServer4.Extensions;
using IdentityServer4.Hosting;
using IdentityServer4.ResponseHandling;
using IdentityServer4.Services;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace IdentityServer4.Endpoints
{
    /// <summary>
    /// The device authorization endpoint
    /// </summary>
    /// <seealso cref="IdentityServer4.Hosting.IEndpointHandler" />
    internal class DeviceAuthorizationEndpoint : IEndpointHandler
    {
        private readonly IClientSecretValidator _clientValidator;
        private readonly IDeviceAuthorizationRequestValidator _requestValidator;
        private readonly IDeviceAuthorizationResponseGenerator _responseGenerator;
        private readonly IEventService _events;
        private readonly ILogger<DeviceAuthorizationEndpoint> _logger;

        public DeviceAuthorizationEndpoint(
            IClientSecretValidator clientValidator,
            IDeviceAuthorizationRequestValidator requestValidator,
            IDeviceAuthorizationResponseGenerator responseGenerator,
            IEventService events,
            ILogger<DeviceAuthorizationEndpoint> logger)
        {
            _clientValidator = clientValidator;
            _requestValidator = requestValidator;
            _responseGenerator = responseGenerator;
            _events = events;
            _logger = logger;
        }

        /// <summary>
        /// Processes the request.
        /// </summary>
        /// <param name="context">The HTTP context.</param>
        /// <returns></returns>
        public async Task<IEndpointResult> ProcessAsync(HttpContext context)
        {
            var request = await ReadRequest(context);
            if (request.IsError)
            {
                return Error(request.Error, request.ErrorDescription);
            }

            var result = await ProcessDeviceAuthorizationRequestAsync(request.ValidatedRequest);
            if (result.IsError)
            {
                return Error(result.Error, result.ErrorDescription);
            }

            LogResponse(result.Response, request.ValidatedRequest);

            return new DeviceAuthorizationResult(result.Response);
        }

        private async Task<ValidatedRequestResult> ReadRequest(HttpContext context)
        {
            // Implementation of ReadRequest method
            return await Task.FromResult(new ValidatedRequestResult());
        }

        private async Task<DeviceAuthorizationResponseValidationResult> ProcessDeviceAuthorizationRequestAsync(ValidatedDeviceAuthorizationRequest request)
        {
            // Implementation of ProcessDeviceAuthorizationRequestAsync method
            return await Task.FromResult(new DeviceAuthorizationResponseValidationResult());
        }

        private TokenErrorResult Error(string error, string errorDescription)
        {
            var response = new IdentityServer4.Models.TokenErrorResponse
            {
                Error = error,
                ErrorDescription = errorDescription
            };

            _logger.LogError("Device authorization error: {error}:{errorDescriptions}", error, error ?? "-no message-");

            return new TokenErrorResult(response);
        }

        private void LogResponse(DeviceAuthorizationResponse response, ValidatedDeviceAuthorizationRequest request)
        {
            var clientId = $"{request.Client.ClientId} ({request.Client?.ClientName ?? "no name set"})";

            if (response.DeviceCode != null)
            {
                _logger.LogTrace("Device code issued for {clientId}: {deviceCode}", clientId, response.DeviceCode);
            }
            if (response.UserCode != null)
            {
                _logger.LogTrace("User code issued for {clientId}: {userCode}", clientId, response.UserCode);
            }
            if (response.VerificationUri != null)
            {
                _logger.LogTrace("Verification URI issued for {clientId}: {verificationUri}", clientId, response.VerificationUri);
            }
            if (response.VerificationUriComplete != null)
            {
                _logger.LogTrace("Verification URI (Complete) issued for {clientId}: {verificationUriComplete}", clientId, response.VerificationUriComplete);
            }
        }
    }
}
