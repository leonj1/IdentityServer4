using System.Security.Claims;

namespace IdentityServer4.Models
{
    public class IdentityServerUser : ClaimsPrincipal
    {
        public IdentityServerUser(string subject)
            : base(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, subject),
                new Claim("sub", subject)
            }))
        {
        }
    }
}
