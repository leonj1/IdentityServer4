using IdentityModel;
using IdentityServer4;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text.Json;

namespace IdentityServerHost.Quickstart.UI
{
    public class ClaimService
    {
        public static List<Claim> GetClaims()
        {
            var address = new
            {
                street_address = "One Hacker Way",
                locality = "Heidelberg",
                postal_code = 69118,
                country = "Germany"
            };

            return new List<Claim>
            {
                new Claim(JwtClaimTypes.Name, "Alice Smith"),
                new Claim(JwtClaimTypes.GivenName, "Alice"),
                new Claim(JwtClaimTypes.FamilyName, "Smith"),
                new Claim(JwtClaimTypes.Email, "AliceSmith@email.com"),
                new Claim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean),
                new Claim(JwtClaimTypes.WebSite, "http://alice.com"),
                new Claim(JwtClaimTypes.Address, JsonSerializer.Serialize(address), IdentityServerConstants.ClaimValueTypes.Json)
            };
        }
    }
}
