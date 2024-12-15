using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;

namespace IdentityServerTests
{
    public class IdentityServerPipeline
    {
        private const string Category = "Authorize endpoint";

        public OnPostConfigureServicesDelegate OnPostConfigureServices { get; set; } = _ => { };

        public List<Client> Clients { get; } = new List<Client>();
        public List<IdentityResource> IdentityScopes { get; } = new List<IdentityResource>();
        public List<IdentityServer4.Test.TestUser> Users { get; } = new List<IdentityServer4.Test.TestUser>();

        private BrowserClient _browserClient;
        private TestIdentityServerPipeline _pipeline;

        public BrowserClient BrowserClient => _browserClient ?? (_browserClient = new BrowserClient());

        public async Task Initialize()
        {
            var services = new ServiceCollection();
            OnPostConfigureServices(services);

            services.AddIdentityServer(options =>
            {
                options.PublicOrigin = "https://idsrv";
            })
            .AddInMemoryClients(Clients)
            .AddInMemoryIdentityResources(IdentityScopes)
            .AddTestUsers(Users)
            .AddDeveloperSigningCredential();

            _pipeline = new TestIdentityServerPipeline(services.BuildServiceProvider());
        }

        public async Task LoginAsync(string username, string password = "password")
        {
            await _pipeline.LoginAsync(username, password);
        }

        public string CreateAuthorizeUrl(
            string clientId,
            string responseType,
            string scope,
            string redirectUri,
            string state,
            string nonce)
        {
            return _pipeline.CreateAuthorizeUrl(clientId, responseType, scope, redirectUri, state, nonce);
        }
    }

    public delegate void OnPostConfigureServicesDelegate(IServiceCollection services);

    public class BrowserClient : IdentityModel.Client.HttpMessageHandlerWrapper
    {
        public bool AllowAutoRedirect { get; set; } = true;

        public BrowserClient()
            : base(new HttpClientHandler())
        {
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (AllowAutoRedirect && request.RequestUri.AbsolutePath.Contains("/signin-oidc"))
            {
                var response = await base.SendAsync(request, cancellationToken);
                if (response.StatusCode == HttpStatusCode.Redirect)
                {
                    var location = response.Headers.Location;
                    var redirectResponse = new HttpResponseMessage(HttpStatusCode.Found)
                    {
                        Headers =
                        {
                            Location = location
                        }
                    };
                    return redirectResponse;
                }

                return response;
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
