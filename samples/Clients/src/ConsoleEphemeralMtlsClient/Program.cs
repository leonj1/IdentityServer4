using System;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace ConsoleEphemeralMtlsClient
{
    class Program
    {
        private static X509Certificate2 ClientCertificate;

        public static async Task Main(string[] args)
        {
            ClientCertificate = CreateClientCertificate("client");

            var response = await RequestTokenAsync();
            response.Show();

            Console.ReadLine();
            await CallServiceAsync(response.AccessToken);
        }
    }
}
