using System.Threading.Tasks;

namespace IdentityServer.IntegrationTests.Common
{
    public class IdentityServerInitializer
    {
        public static async Task Initialize(IdentityServerPipeline pipeline)
        {
            pipeline.Handler = new HttpClientHandler();
            pipeline.BrowserClient = new BrowserClient(pipeline.Handler);
        }
    }
}
