using Microsoft.Extensions.DependencyInjection;

namespace IdentityServer4.Configuration
{
    /// <summary>
    /// Extensions for the IdentityServer builder.
    /// </summary>
    public static class IdentityServerBuilderExtensions
    {
        /// <summary>
        /// Creates a builder.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <returns></returns>
        public static IIdentityServerBuilder AddIdentityServerBuilder(this IServiceCollection services)
        {
            return new IdentityServerBuilder(services);
        }
    }
}
