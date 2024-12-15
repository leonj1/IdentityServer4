using IdentityModel;
using IdentityServer4.Extensions;
using System;

namespace IdentityServer4.Validation
{
    public static class ValidatedAuthorizeRequestExtensionsHelper
    {
        public static void RemovePrompt(this ValidatedAuthorizeRequest request)
        {
            request.PromptModes = Enumerable.Empty<string>();
            request.Raw.Remove(OidcConstants.AuthorizeRequest.Prompt);
        }
    }
}
