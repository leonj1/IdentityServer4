// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System.Linq;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;

namespace IdentityServer4.Validation
{
    /// <summary>
    /// Validates an incoming token request using the device flow
    /// </summary>
    internal class DeviceCodeValidator : IDeviceCodeValidator
    {
        private readonly IDeviceFlowCodeService _devices;
        private readonly IProfileService _profile;
        private readonly IDeviceFlowThrottlingService _throttlingService;
        private readonly ISystemClock _systemClock;
        private readonly ILogger<DeviceCodeValidator> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceCodeValidator"/> class.
        /// </summary>
        /// <param name="devices">The devices.</param>
        /// <param name="profile">The profile.</param>
        /// <param name="throttlingService">The throttling service.</param>
        /// <param name="systemClock">The system clock.</param>
        /// <param name="logger">The logger.</param>
        public DeviceCodeValidator(
            IDeviceFlowCodeService devices,
            IProfileService profile,
            IDeviceFlowThrottlingService throttlingService,
            ISystemClock systemClock,
            ILogger<DeviceCodeValidator> logger)
        {
            _devices = devices;
            _profile = profile;
            _throttlingService = throttlingService;
            _systemClock = systemClock;
            _logger = logger;
        }

        /// <summary>
        /// Validates the device code.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public async Task ValidateAsync(TokenRequestValidationContext context)
        {
            var deviceCode = context.DeviceCode;

            // validate lifetime
            if (deviceCode.CreationTime.AddSeconds(deviceCode.Lifetime) < _systemClock.UtcNow)
            {
                _logger.LogError("Expired device code");
                context.Result = new TokenRequestValidationResult(context.Request, OidcConstants.TokenErrors.ExpiredToken);
                return;
            }

            // denied
            if (deviceCode.IsAuthorized
                && (deviceCode.AuthorizedScopes == null || deviceCode.AuthorizedScopes.Any() == false))
            {
                _logger.LogError("No scopes authorized for device authorization. Access denied");
                context.Result = new TokenRequestValidationResult(context.Request, OidcConstants.TokenErrors.AuthorizationPending);
                return;
            }

            // make sure user is enabled
            var isActiveCtx = new IsActiveContext(deviceCode.Subject, context.Request.Client, IdentityServerConstants.ProfileIsActiveCallers.DeviceCodeValidation);
            await _profile.IsActiveAsync(isActiveCtx);

            if (isActiveCtx.IsActive == false)
            {
                _logger.LogError("User has been disabled: {subjectId}", deviceCode.Subject.GetSubjectId());
                context.Result = new TokenRequestValidationResult(context.Request, OidcConstants.TokenErrors.InvalidGrant);
                return;
            }

            context.Request.DeviceCode = deviceCode;
            context.Request.SessionId = deviceCode.SessionId;

            context.Result = new TokenRequestValidationResult(context.Request);
            await _devices.RemoveByDeviceCodeAsync(context.DeviceCode);
        }
    }
}
