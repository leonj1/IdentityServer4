using IdentityServer4.Models;

namespace YourNamespace // Replace with your actual namespace
{
    public static class TokenErrorConverter
    {
        private static string ConvertTokenErrorEnumToString(TokenRequestErrors error)
        {
            return error switch
            {
                TokenRequestErrors.InvalidClient => OidcConstants.TokenErrors.InvalidClient,
                TokenRequestErrors.InvalidGrant => OidcConstants.TokenErrors.InvalidGrant,
                TokenRequestErrors.InvalidRequest => OidcConstants.TokenErrors.InvalidRequest,
                TokenRequestErrors.InvalidScope => OidcConstants.TokenErrors.InvalidScope,
                TokenRequestErrors.UnauthorizedClient => OidcConstants.TokenErrors.UnauthorizedClient,
                TokenRequestErrors.UnsupportedGrantType => OidcConstants.TokenErrors.UnsupportedGrantType,
                TokenRequestErrors.InvalidTarget => OidcConstants.TokenErrors.InvalidTarget,
                _ => throw new InvalidOperationException("invalid token error")
            };
        }
    }
}
