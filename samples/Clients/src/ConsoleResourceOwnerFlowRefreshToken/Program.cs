using Clients;
using IdentityModel;
using IdentityModel.Client;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleResourceOwnerFlowRefreshToken
{
    public class Program
    {
        static HttpClient _tokenClient = new HttpClient();
        static DiscoveryCache _cache = new DiscoveryCache(Constants.Authority);

        static async Task Main()
        {
            Console.Title = "Console ResourceOwner Flow RefreshToken";

            var response = await RequestTokenAsync();
            response.Show();

            Console.ReadLine();

            var refresh_token = response.RefreshToken;

            while (true)
            {
                response = await RefreshTokenAsync(refresh_token);
                ShowResponse(response);

                Console.ReadLine();
                await CallServiceAsync(response.AccessToken);

                if (response.RefreshToken != refresh_token)
                {
                    refresh_token = response.RefreshToken;
                }
            }
        }

        static async Task<TokenResponse> RequestTokenAsync()
        {
            var disco = await _cache.GetAsync();

            var response = await _tokenClient.RequestPasswordTokenAsync(new PasswordTokenRequest
            {
                Address = disco.TokenEndpoint,

                ClientId = "roclient",
                ClientSecret = "secret",

                UserName = "bob",
                Password = "bob",

                Scope = "resource1.scope1 offline_access",
            });

            if (response.IsError) throw new Exception(response.Error);
            return response;
        }

        private static async Task<TokenResponse> RefreshTokenAsync(string refreshToken)
        {
            Console.WriteLine("Using refresh token: {0}", refreshToken);

            var disco = await _cache.GetAsync();
            var response = await _tokenClient.RequestRefreshTokenAsync(new RefreshTokenRequest
            {
                Address = disco.TokenEndpoint,

                ClientId = "roclient",
                ClientSecret = "secret",
                RefreshToken = refreshToken
            });

            if (response.IsError) throw new Exception(response.Error);
            return response;
        }

        static async Task CallServiceAsync(string token)
        {
            var baseAddress = Constants.SampleApi;

            using (var client = new HttpClientWrapper(baseAddress))
            {
                await client.CallServiceAsync(token);
            }
        }

        private static void ShowResponse(TokenResponse response)
        {
            if (!response.IsError)
            {
                "Token response:".ConsoleGreen();
                Console.WriteLine(response.Json);

                if (response.AccessToken.Contains("."))
                {
                    "\nAccess Token (decoded):".ConsoleGreen();

                    var parts = response.AccessToken.Split('.');
                    var header = parts[0];
                    var claims = parts[1];

                    Console.WriteLine(JObject.Parse(Encoding.UTF8.GetString(Base64Url.Decode(header))));
                    Console.WriteLine(JObject.Parse(Encoding.UTF8.GetString(Base64Url.Decode(claims))));
                }
            }
            else
            {
                if (response.ErrorType == ResponseErrorType.Http)
                {
                    "HTTP error: ".ConsoleGreen();
                    Console.WriteLine(response.Error);
                    "HTTP status code: ".ConsoleGreen();
                    Console.WriteLine(response.HttpStatusCode);
                }
                else
                {
                    "Protocol error response:".ConsoleGreen();
                    Console.WriteLine(response.Json);
                }
            }
        }
    }

    public class HttpClientWrapper : IDisposable
    {
        private readonly HttpClient _httpClient;

        public HttpClientWrapper(string baseAddress)
        {
            _httpClient = new HttpClient { BaseAddress = new Uri(baseAddress) };
        }

        public async Task CallServiceAsync(string token)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "api/resource");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            using (var response = await _httpClient.SendAsync(request))
            {
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"HTTP error: {response.StatusCode}");
                }

                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine(content);
            }
        }

        public void Dispose()
        {
            _httpClient.Dispose();
        }
    }
}
