using Microsoft.AspNetCore.Builder;

namespace IdentityServerExample
{
    public class IdentityServerConfig
    {
        public static void Configure(IApplicationBuilder app)
        {
            // Existing code for IdentityServerPipeline
            app.UseIdentityServer();
        }
    }
}
