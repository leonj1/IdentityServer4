using IdentityServer4.Configuration;
using IdentityServer4.Validation;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityServer.IntegrationTests.Clients.Setup
{
    public static class IdentityServerConfig
    {
        public static void ConfigureServices(IServiceCollection builder)
        {
            builder.AddInMemoryClients(Clients.Get());
            builder.AddInMemoryIdentityResources(Scopes.GetIdentityScopes());
            builder.AddInMemoryApiResources(Scopes.GetApiResources());
            builder.AddInMemoryApiScopes(Scopes.GetApiScopes());
            builder.AddTestUsers(Users.Get());

            builder.AddDeveloperSigningCredential(persistKey: false);

            builder.AddExtensionGrantValidator<ExtensionGrantValidator>();
            builder.AddExtensionGrantValidator<ExtensionGrantValidator2>();
            builder.AddExtensionGrantValidator<NoSubjectExtensionGrantValidator>();
            builder.AddExtensionGrantValidator<DynamicParameterExtensionGrantValidator>();

            builder.AddProfileService<CustomProfileService>();
        }
    }
}
