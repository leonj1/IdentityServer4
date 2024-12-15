using Microsoft.AspNetCore.Identity;

namespace YourNamespace
{
    public static class ProfileServiceExtensions
    {
        public static async Task GetProfileDataAsync(this ProfileService<IdentityUser> profileService, ProfileDataRequestContext context)
        {
            var user = await profileService.FindUserAsync(context.Subject.GetSubjectId());
            if (user != null)
            {
                await profileService.GetProfileDataAsync(context, user);
            }
        }

        protected static async Task GetProfileDataAsync(this ProfileService<IdentityUser> profileService, ProfileDataRequestContext context, IdentityUser user)
        {
            var principal = await profileService.GetUserClaimsAsync(user);
            context.AddRequestedClaims(principal.Claims);
        }
    }
}
