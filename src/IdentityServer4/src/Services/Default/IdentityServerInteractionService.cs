// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IdentityServerInteraction
{
    public class IdentityServerInteractionService : DefaultIdentityServerInteractionService
    {
        // Constructor and methods remain the same as in the original file
        public IdentityServerInteractionService(IIdentityServerInteractionService inner) : base(inner)
        {
        }

        public override async Task<AuthorizationRequest> GetAuthorizationContextAsync(string returnUrl)
        {
            return await base.GetAuthorizationContextAsync(returnUrl);
        }

        public override async Task<ConsentResponse> ProcessConsentAsync(ConsentRequest request, ConsentViewModel model)
        {
            return await base.ProcessConsentAsync(request, model);
        }

        // Other methods remain the same
    }
}
