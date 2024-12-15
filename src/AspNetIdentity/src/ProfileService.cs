using System.Security.Claims;
using System.Threading.Tasks;

namespace YourNamespace
{
    public class ProfileService<TUser>
    {
        private readonly UserManager<TUser> _userManager;
        private readonly IUserClaimsPrincipalFactory<TUser> _claimsFactory;

        public ProfileService(UserManager<TUser> userManager, IUserClaimsPrincipalFactory<TUser> claimsFactory)
        {
            _userManager = userManager;
            _claimsFactory = claimsFactory;
        }

        public virtual async Task<ClaimsPrincipal> GetUserClaimsAsync(TUser user)
        {
            var principal = await _claimsFactory.CreateAsync(user);
            if (principal == null) throw new Exception("ClaimsFactory failed to create a principal");

            return principal;
        }

        public virtual async Task IsActiveAsync(IsActiveContext context, string subjectId)
        {
            var user = await FindUserAsync(subjectId);
            if (user != null)
            {
                await IsActiveAsync(context, user);
            }
            else
            {
                context.IsActive = false;
            }
        }

        protected virtual async Task IsActiveAsync(IsActiveContext context, TUser user)
        {
            context.IsActive = await IsUserActiveAsync(user);
        }

        public virtual Task<bool> IsUserActiveAsync(TUser user)
        {
            return Task.FromResult(true);
        }

        protected virtual async Task<TUser> FindUserAsync(string subjectId)
        {
            var user = await _userManager.FindByIdAsync(subjectId);
            if (user == null)
            {
                // Logger?.LogWarning("No user found matching subject Id: {subjectId}", subjectId);
            }

            return user;
        }
    }
}
