using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace IdentityServerTests
{
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
