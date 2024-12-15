using Clients;
using IdentityModel.Client;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace ConsoleResourceOwnerFlowPublic
{
    public class Program
    {
        static async Task Main()
        {
            Console.Title = "Console ResourceOwner Flow Public";

            var response = await RequestTokenAsync();
            response.Show();

            Console.ReadLine();
            await CallServiceAsync(response.AccessToken);
        }
    }
}
