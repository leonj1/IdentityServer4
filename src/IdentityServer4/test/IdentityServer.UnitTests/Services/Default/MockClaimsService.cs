using System.Collections.Generic;
using IdentityModel;
using IdentityServer4.Models;

namespace IdentityServer.UnitTests.Common
{
    public class MockClaimsService : IClaimsService
    {
        public Task<IEnumerable<Claim>> GetClaimsAsync(IEnumerable<string> scopes)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtClaimTypes.Name, "Test User"),
                new Claim(JwtClaimTypes.Email, "test@example.com")
            };

            return Task.FromResult(claims);
        }
    }
}
