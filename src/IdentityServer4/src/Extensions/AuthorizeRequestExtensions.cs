// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityModel;
using IdentityServer4.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

#pragma warning disable 1591

namespace IdentityServer4.Validation
{
    public static class AuthorizeRequestExtensions
    {
        public static void RemovePrompt(this ValidatedAuthorizeRequest request)
        {
            request.PromptModes = Enumerable.Empty<string>();
            request.Raw.Remove(OidcConstants.AuthorizeRequest.Prompt);
        }
    }
}
