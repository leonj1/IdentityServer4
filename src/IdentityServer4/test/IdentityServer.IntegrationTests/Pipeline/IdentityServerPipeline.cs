using System.Collections.Generic;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Test;

namespace IdentityServer.IntegrationTests.Common
{
    public class IdentityServerPipeline
    {
        public const string DiscoveryEndpoint = "/discovery/.well-known/openid-configuration";
        public const string DiscoveryKeysEndpoint = "/discovery/jwks.json";
        public const string TokenEndpoint = "/connect/token";
        public const string UserInfoEndpoint = "/connect/userinfo";
        public const string RevocationEndpoint = "/connect/revocation";
        public const string AuthorizeEndpoint = "/connect/authorize";
        public const string EndSessionEndpoint = "/connect/endsession";
        public const string CheckSessionEndpoint = "/connect/checksession";
        public const string LoginPage = "/Account/Login";
        public const string ConsentPage = "/Consent/Index";
        public const string ErrorPage = "/Home/Error";

        public List<Client> Clients { get; set; } = new List<Client>();
        public List<TestUser> Users { get; set; } = new List<TestUser>();
        public List<IdentityResource> IdentityScopes { get; set; } = new List<IdentityResource>();
        public List<ApiResource> ApiResources { get; set; } = new List<ApiResource>();
        public List<ApiScope> ApiScopes { get; set; } = new List<ApiScope>();

        public HttpClient BackChannelClient { get; private set; }

        public Action<IServiceCollection> OnPreConfigureServices { get; set; }

        public IdentityServerPipeline()
        {
            BackChannelClient = new HttpClient();
        }

        public void Initialize()
        {
            var services = new ServiceCollection();

            if (OnPreConfigureServices != null)
                OnPreConfigureServices(services);

            services.AddIdentityServer(options =>
            {
                options.Clients.AddRange(Clients);
                options.IdentityResources.AddRange(IdentityScopes);
                options.ApiResources.AddRange(ApiResources);
                options.ApiScopes.AddRange(ApiScopes);
            });

            var serviceProvider = services.BuildServiceProvider();

            BackChannelClient.BaseAddress = new System.Uri("http://localhost:5000");
        }
    }
}
