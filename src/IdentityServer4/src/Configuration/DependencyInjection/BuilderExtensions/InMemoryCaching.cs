using IdentityServer4.Services;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace IdentityServer4.Configuration
{
    /// <summary>
    /// Extension methods for configuring in-memory caching.
    /// </summary>
    public static class InMemoryCaching
    {
        /// <summary>
        /// Adds in-memory caching to the identity server builder.
        /// </summary>
        /// <param name="builder">The identity server builder.</param>
        /// <returns>The identity server builder.</returns>
        public static IIdentityServerBuilder AddInMemoryCaching(this IIdentityServerBuilder builder)
        {
            builder.Services.TryAddSingleton<ICacheService, MemoryCacheService>();

            return builder;
        }
    }
}
