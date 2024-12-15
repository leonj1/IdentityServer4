using Clients;
using IdentityModel;
using IdentityModel.Client;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace ConsoleMTLSClient
{
    public class Program
    {
        public static async Task Main()
        {
            Console.Title = "Console mTLS Client";

            var response = await RequestTokenAsync();
            response.Show();

            Console.ReadLine();
            await CallServiceAsync(response.AccessToken);
        }
    }
}
