using System;
using System.Collections.Generic;
using System.Linq;
using IdentityServer4.Models;

namespace YourNamespace // Replace with your actual namespace
{
    public class GrantValidationResult
    {
        public bool IsError { get; set; }
        public string Error { get; set; }
        public string ErrorDescription { get; set; }
        public ClaimsPrincipal Subject { get; set; }
        public Dictionary<string, object> CustomResponse { get; set; }

        public GrantValidationResult()
        {
        }

        public GrantValidationResult(
            string subject,
            string authenticationMethod,
            DateTime authTime,
            IEnumerable<Claim> claims = null,
            string identityProvider = IdentityServerConstants.DefaultIdentityProvider,
            Dictionary<string, object> customResponse = null)
        {
            IsError = false;

            var resultClaims = new List<Claim>
            {
                new Claim(JwtClaimTypes.Subject, subject),
                new Claim(JwtClaimTypes.AuthenticationMethod, authenticationMethod),
                new Claim(JwtClaimTypes.IdentityProvider, identityProvider),
                new Claim(JwtClaimTypes.AuthenticationTime, new DateTimeOffset(authTime).ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
            };

            if (!claims.IsNullOrEmpty())
            {
                resultClaims.AddRange(claims);
            }

            var id = new ClaimsIdentity(authenticationMethod);
            id.AddClaims(resultClaims.Distinct(new ClaimComparer()));

            Subject = new ClaimsPrincipal(id);
            CustomResponse = customResponse;
        }
    }
}
