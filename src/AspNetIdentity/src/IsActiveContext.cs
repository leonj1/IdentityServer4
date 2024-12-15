using System.Security.Claims;

namespace YourNamespace
{
    public class IsActiveContext
    {
        public ClaimsPrincipal Subject { get; set; }
        public bool IsActive { get; set; }
    }
}
