using IdentityServer4.Models;

namespace IdentityServer4.Extensions
{
    public static class IdentityResourceExtensions
    {
        /// <summary>
        /// Finds the IdentityResource that matches the scope.
        /// </summary>
        /// <param name="resources">The resources.</param>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public static IdentityResource FindIdentityResourcesByScope(this Resources resources, string name)
        {
            var q = from id in resources.IdentityResources
                    where id.Name == name
                    select id;
            return q.FirstOrDefault();
        }
    }
}
