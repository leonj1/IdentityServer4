// IdentityServer.Services/IdentityServerAuthenticationService.cs
namespace IdentityServer.Services
{
    using Microsoft.AspNetCore.Http;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;

    public class IdentityServerAuthenticationService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public IdentityServerAuthenticationService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<AuthenticateResult> AuthenticateAsync(string scheme)
        {
            return await _httpContextAccessor.HttpContext.AuthenticateAsync(scheme);
        }

        public Task ChallengeAsync(string scheme, AuthenticationProperties properties)
        {
            return _httpContextAccessor.HttpContext.ChallengeAsync(scheme, properties);
        }

        public Task ForbidAsync(string scheme, AuthenticationProperties properties)
        {
            return _httpContextAccessor.HttpContext.ForbidAsync(scheme, properties);
        }

        private void AssertRequiredClaims(ClaimsPrincipal principal)
        {
            // for now, we don't allow more than one identity in the principal/cookie
            if (principal.Identities.Count() != 1) throw new InvalidOperationException("only a single identity supported");
            if (principal.FindFirst(JwtClaimTypes.Subject) == null) throw new InvalidOperationException("sub claim is missing");
        }

        private void AugmentMissingClaims(ClaimsPrincipal principal, DateTime authTime)
        {
            var identity = principal.Identities.First();

            // ASP.NET Identity issues this claim type and uses the authentication middleware name
            // such as "Google" for the value. this code is trying to correct/convert that for
            // our scenario. IOW, we take their old AuthenticationMethod value of "Google"
            // and issue it as the idp claim. we then also issue a amr with "external"
            var amr = identity.FindFirst(ClaimTypes.AuthenticationMethod);
            if (amr != null)
            {
                if (amr.Value == IdentityServerConstants.LocalIdentityProvider)
                {
                    _logger.LogDebug("Adding amr claim with value: {value}", OidcConstants.AuthenticationMethods.Password);
                    identity.AddClaim(new Claim(JwtClaimTypes.AuthenticationMethod, OidcConstants.AuthenticationMethods.Password));
                }
                else
                {
                    _logger.LogDebug("Adding amr claim with value: {value}", Constants.ExternalAuthenticationMethod);
                    identity.AddClaim(new Claim(JwtClaimTypes.AuthenticationMethod, Constants.ExternalAuthenticationMethod));
                }
            }

            if (identity.FindFirst(JwtClaimTypes.AuthenticationTime) == null)
            {
                var time = new DateTimeOffset(authTime).ToUnixTimeSeconds().ToString();

                _logger.LogDebug("Adding auth_time claim with value: {value}", time);
                identity.AddClaim(new Claim(JwtClaimTypes.AuthenticationTime, time, ClaimValueTypes.Integer64));
            }
        }

        public Task AuthenticateAsync(HttpContext context, string scheme)
        {
            return context.AuthenticateAsync(scheme);
        }

        public Task ChallengeAsync(HttpContext context, string scheme, AuthenticationProperties properties)
        {
            return context.ChallengeAsync(scheme, properties);
        }

        public Task ForbidAsync(HttpContext context, string scheme, AuthenticationProperties properties)
        {
            return context.ForbidAsync(scheme, properties);
        }
    }
}
