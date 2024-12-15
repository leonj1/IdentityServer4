// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Validation;

namespace YourNamespace // Replace with your actual namespace
{
    public class DeviceAuthorizationRequestValidator
    {
        private readonly IResourceValidationService _resourceValidationService;
        private readonly InputLengthRestrictions _options;

        public DeviceAuthorizationRequestValidator(IResourceValidationService resourceValidationService, InputLengthRestrictions options)
        {
            _resourceValidationService = resourceValidationService;
            _options = options;
        }

        public async Task<DeviceAuthorizationRequestValidationResult> ValidateAsync(NameValueCollection parameters, Client client)
        {
            var request = new ValidatedDeviceAuthorizationRequest
            {
                Raw = parameters,
                SetClient(client),
                IsOpenIdRequest = false,
                RequestedScopes = new List<string>()
            };

            //////////////////////////////////////////////////////////
            // scope must be present
            //////////////////////////////////////////////////////////
            var scope = request.Raw.Get(OidcConstants.AuthorizeRequest.Scope);
            if (scope.IsMissing())
            {
                _logger.LogTrace("Client provided no scopes - checking allowed scopes list");

                if (!client.AllowedScopes.IsNullOrEmpty())
                {
                    var clientAllowedScopes = new List<string>(client.AllowedScopes);
                    if (client.AllowOfflineAccess)
                    {
                        clientAllowedScopes.Add(IdentityServerConstants.StandardScopes.OfflineAccess);
                    }
                    scope = clientAllowedScopes.ToSpaceSeparatedString();
                    _logger.LogTrace("Defaulting to: {scopes}", scope);
                }
                else
                {
                    LogError("No allowed scopes configured for client", request);
                    return Invalid(request, OidcConstants.AuthorizeErrors.InvalidScope);
                }
            }

            if (scope.Length > _options.InputLengthRestrictions.Scope)
            {
                LogError("scopes too long.", request);
                return Invalid(request, description: "Invalid scope");
            }

            request.RequestedScopes = scope.FromSpaceSeparatedString().Distinct().ToList();

            if (request.RequestedScopes.Any(s => s == IdentityServerConstants.StandardScopes.OpenId))
            {
                request.IsOpenIdRequest = true;
            }

            request.ValidatedResources = await _resourceValidationService.ValidateIdentityResourcesAsync(request.RequestedScopes);

            if (request.ValidatedResources.Resources.IdentityResources.Any() && !request.IsOpenIdRequest)
            {
                LogError("Identity related scope requests, but no openid scope", request);
                return Invalid(request, OidcConstants.AuthorizeErrors.InvalidScope);
            }

            return Valid(request);
        }

        private DeviceAuthorizationRequestValidationResult Valid(ValidatedDeviceAuthorizationRequest request)
        {
            return new DeviceAuthorizationRequestValidationResult
            {
                IsError = false,
                Request = request
            };
        }

        private DeviceAuthorizationRequestValidationResult Invalid(ValidatedDeviceAuthorizationRequest request, string error, string description = null)
        {
            _logger.LogError(error);
            if (description != null)
            {
                _logger.LogDebug(description);
            }
            return new DeviceAuthorizationRequestValidationResult
            {
                IsError = true,
                Error = error,
                Description = description
            };
        }

        private void LogError(string message, params object[] args)
        {
            _logger.LogError(message, args);
        }

        private void LogError(ValidatedDeviceAuthorizationRequest request, string message)
        {
            _logger.LogError(message);
        }
    }
}
