using System;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityModel;
using IdentityServer4.Configuration;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Xunit;

namespace IdentityServer4.UnitTests.Validation
{
    public class BasicAuthenticationSecretParserTests
    {
        private readonly IdentityServerOptions _options;
        private readonly BasicAuthenticationSecretParser _parser;
        private readonly ILogger<BasicAuthenticationSecretParser> _logger;

        public BasicAuthenticationSecretParserTests()
        {
            _options = new IdentityServerOptions();
            _logger = new LoggerFactory().CreateLogger<BasicAuthenticationSecretParser>();
            _parser = new BasicAuthenticationSecretParser(_options, _logger);
        }

        [Fact]
        public void AuthenticationMethod_Should_Return_Basic()
        {
            _parser.AuthenticationMethod.Should().Be(OidcConstants.EndpointAuthenticationMethods.BasicAuthentication);
        }

        [Fact]
        public async Task Valid_Basic_Authentication_Header_Should_Parse_Correctly()
        {
            // Arrange
            var context = new DefaultHttpContext();
            var credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes("client:secret"));
            context.Request.Headers.Add("Authorization", $"Basic {credentials}");

            // Act
            var secret = await _parser.ParseAsync(context);

            // Assert
            secret.Should().NotBeNull();
            secret.Id.Should().Be("client");
            secret.Credential.Should().Be("secret");
            secret.Type.Should().Be(IdentityServerConstants.ParsedSecretTypes.SharedSecret);
        }

        [Fact]
        public async Task Missing_Authorization_Header_Should_Return_Null()
        {
            // Arrange
            var context = new DefaultHttpContext();

            // Act
            var secret = await _parser.ParseAsync(context);

            // Assert
            secret.Should().BeNull();
        }

        [Fact]
        public async Task Non_Basic_Authorization_Header_Should_Return_Null()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Request.Headers.Add("Authorization", "Bearer something");

            // Act
            var secret = await _parser.ParseAsync(context);

            // Assert
            secret.Should().BeNull();
        }

        [Fact]
        public async Task Malformed_Basic_Authentication_Credential_Should_Return_Null()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Request.Headers.Add("Authorization", "Basic invalid_base64");

            // Act
            var secret = await _parser.ParseAsync(context);

            // Assert
            secret.Should().BeNull();
        }

        [Fact]
        public async Task Missing_Colon_In_Credentials_Should_Return_Null()
        {
            // Arrange
            var context = new DefaultHttpContext();
            var credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes("clientsecret"));
            context.Request.Headers.Add("Authorization", $"Basic {credentials}");

            // Act
            var secret = await _parser.ParseAsync(context);

            // Assert
            secret.Should().BeNull();
        }

        [Fact]
        public async Task Credentials_Exceeding_Length_Restrictions_Should_Return_Null()
        {
            // Arrange
            var context = new DefaultHttpContext();
            var longClientId = new string('x', _options.InputLengthRestrictions.ClientId + 1);
            var credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{longClientId}:secret"));
            context.Request.Headers.Add("Authorization", $"Basic {credentials}");

            // Act
            var secret = await _parser.ParseAsync(context);

            // Assert
            secret.Should().BeNull();
        }

        [Fact]
        public async Task Client_Without_Secret_Should_Parse_As_NoSecret()
        {
            // Arrange
            var context = new DefaultHttpContext();
            var credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes("client:"));
            context.Request.Headers.Add("Authorization", $"Basic {credentials}");

            // Act
            var secret = await _parser.ParseAsync(context);

            // Assert
            secret.Should().NotBeNull();
            secret.Id.Should().Be("client");
            secret.Type.Should().Be(IdentityServerConstants.ParsedSecretTypes.NoSecret);
        }

        [Fact]
        public async Task Url_Encoded_Credentials_Should_Be_Decoded()
        {
            // Arrange
            var context = new DefaultHttpContext();
            var credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes("client+with+spaces:secret+with+spaces"));
            context.Request.Headers.Add("Authorization", $"Basic {credentials}");

            // Act
            var secret = await _parser.ParseAsync(context);

            // Assert
            secret.Should().NotBeNull();
            secret.Id.Should().Be("client with spaces");
            secret.Credential.Should().Be("secret with spaces");
        }
    }
}
