using IdentityServer4.Models;
using System.Collections.Generic;

namespace TokenValidationTests
{
    public class TestProfileService : IProfileService
    {
        public Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            // Implement profile data retrieval logic here
            return Task.CompletedTask;
        }

        public Task IsActiveAsync(IsActiveContext context)
        {
            // Implement active user check logic here
            return Task.CompletedTask;
        }
    }
}
