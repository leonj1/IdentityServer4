using System.Security.Claims;
using IdentityModel;

namespace IdentityServer.IntegrationTests.Clients.Setup
{
    public static class ClaimHelper
    {
        public static List<Claim> GetClaims(string name, string givenName, string familyName, string email, bool emailVerified, string role1, string role2, string website)
        {
            return new List<Claim>
            {
                new Claim(JwtClaimTypes.Name, name),
                new Claim(JwtClaimTypes.GivenName, givenName),
                new Claim(JwtClaimTypes.FamilyName, familyName),
                new Claim(JwtClaimTypes.Email, email),
                new Claim(JwtClaimTypes.EmailVerified, emailVerified.ToString(), ClaimValueTypes.Boolean),
                new Claim(JwtClaimTypes.Role, role1),
                new Claim(JwtClaimTypes.Role, role2),
                new Claim(JwtClaimTypes.WebSite, website),
                new Claim(JwtClaimTypes.Address, @"{ 'street_address': 'One Hacker Way', 'locality': 'Heidelberg', 'postal_code': 69118, 'country': 'Germany' }", IdentityServerConstants.ClaimValueTypes.Json)
            };
        }
    }
}
