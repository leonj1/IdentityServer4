using System.IO;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer.UnitTests.Validation.Setup;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace IdentityServer.UnitTests.Validation.Secrets
{
    public class ClientSecretValidationTests
    {
        private const string Category = "Secrets - Client Secret Validator";

        [Fact]
        [Trait("Category", Category)]
        public async Task confidential_client_with_incorrect_secret_should_not_be_able_to_request_token()
        {
            var validator = Factory.CreateClientSecretValidator();

            var context = new DefaultHttpContext();
            var body = "client_id=roclient&client_secret=invalid";

            context.Request.Body = new MemoryStream(Encoding.UTF8.GetBytes(body));
            context.Request.ContentType = "application/x-www-form-urlencoded";

            var result = await validator.ValidateAsync(context);

            result.IsError.Should().BeTrue();
        }
    }
}
