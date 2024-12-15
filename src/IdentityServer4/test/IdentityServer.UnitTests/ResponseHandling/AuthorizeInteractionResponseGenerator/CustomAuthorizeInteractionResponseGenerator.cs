// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System.Threading.Tasks;
using IdentityServer4;
using IdentityServer4.Configuration;
using IdentityServer4.Models;
using IdentityServer4.ResponseHandling;
using IdentityServer4.Services;
using IdentityServer4.Validation;

namespace IdentityServer.UnitTests.ResponseHandling.AuthorizeInteractionResponseGenerator
{
    public class CustomAuthorizeInteractionResponseGenerator : IdentityServer4.ResponseHandling.AuthorizeInteractionResponseGenerator
    {
        public CustomAuthorizeInteractionResponseGenerator(ISystemClock clock, ILogger<IdentityServer4.ResponseHandling.AuthorizeInteractionResponseGenerator> logger, IConsentService consent, IProfileService profile) : base(clock, logger, consent, profile)
        {
        }

        public InteractionResponse ProcessLoginResponse { get; set; }
        protected internal override Task<InteractionResponse> ProcessLoginAsync(ValidatedAuthorizeRequest request)
        {
            if (ProcessLoginResponse != null)
            {
                return Task.FromResult(ProcessLoginResponse);
            }

            return base.ProcessLoginAsync(request);
        }

        public InteractionResponse ProcessConsentResponse { get; set; }
        protected internal override Task<InteractionResponse> ProcessConsentAsync(ValidatedAuthorizeRequest request, ConsentResponse consent)
        {
            if (ProcessConsentResponse != null)
            {
                return Task.FromResult(ProcessConsentResponse);
            }

            return base.ProcessConsentAsync(request, consent);
        }
    }
}
