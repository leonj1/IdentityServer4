using System.IO;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer.UnitTests.Common;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace IdentityServer.UnitTests.Validation
{
    public class BearerTokenUsageValidationTests
    {
        [Fact]
        [Trait("Category", "BearerTokenUsageValidator Tests")]
        public async Task No_Header_no_Body_Get()
        {
            var ctx = new DefaultHttpContext();
            ctx.Request.Method = "GET";

            var validator = new BearerTokenUsageValidator(TestLogger.Create<BearerTokenUsageValidator>());
            var result = await validator.ValidateAsync(ctx);

            result.TokenFound.Should().BeFalse();
        }
    }
}
