using System.Threading.Tasks;

namespace IdentityServer.UnitTests.Common
{
    public static class UserSessionExtensions
    {
        public static async Task RemoveSessionIdCookieAsync(this IUserSession userSession)
        {
            await ((MockUserSession)userSession).RemoveSessionIdCookieAsync();
        }
    }
}
