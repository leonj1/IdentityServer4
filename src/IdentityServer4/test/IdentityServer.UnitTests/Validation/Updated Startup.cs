using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        // Other service configurations...

        IdentityServerConfig.ConfigureIdentityServerOptions(services);
    }
}
