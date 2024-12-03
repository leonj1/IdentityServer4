using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer4.Configuration;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Xunit;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace IdentityServer4.UnitTests.Validation
{
    public class PostBodySecretParserTests
    {
        private readonly IdentityServerOptions _options;
        private readonly PostBodySecretParser _parser;

        public PostBodySecretParserTests()
        {
            _options = new IdentityServerOptions();
            _parser = new PostBodySecretParser(_options, new LoggerFactory().CreateLogger<PostBodySecretParser>());
        }

        [Fact]
        public async Task Valid_Client_Id_And_Secret_Should_Parse_Successfully()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Request.ContentType = "application/x-www-form-urlencoded";
            context.Request.Form = new FormCollection(new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>
            {
                { "client_id", "client" },
                { "client_secret", "secret" }
            });

            // Act
            var secret = await _parser.ParseAsync(context);

            // Assert
            secret.Should().NotBeNull();
            secret.Id.Should().Be("client");
            secret.Credential.Should().Be("secret");
            secret.Type.Should().Be(IdentityServerConstants.ParsedSecretTypes.SharedSecret);
        }

        [Fact]
        public async Task Valid_Client_Id_Without_Secret_Should_Parse_Successfully()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Request.ContentType = "application/x-www-form-urlencoded";
            context.Request.Form = new FormCollection(new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>
            {
                { "client_id", "client" }
            });

            // Act
            var secret = await _parser.ParseAsync(context);

            // Assert
            secret.Should().NotBeNull();
            secret.Id.Should().Be("client");
            secret.Type.Should().Be(IdentityServerConstants.ParsedSecretTypes.NoSecret);
        }

        [Fact]
        public async Task Invalid_Content_Type_Should_Return_Null()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Request.ContentType = "application/json";

            // Act
            var secret = await _parser.ParseAsync(context);

            // Assert
            secret.Should().BeNull();
        }

        [Fact]
        public async Task Oversized_Client_Id_Should_Return_Null()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Request.ContentType = "application/x-www-form-urlencoded";
            context.Request.Form = new FormCollection(new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>
            {
                { "client_id", new string('x', _options.InputLengthRestrictions.ClientId + 1) },
                { "client_secret", "secret" }
            });

            // Act
            var secret = await _parser.ParseAsync(context);

            // Assert
            secret.Should().BeNull();
        }

        [Fact]
        public async Task Oversized_Client_Secret_Should_Return_Null()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Request.ContentType = "application/x-www-form-urlencoded";
            context.Request.Form = new FormCollection(new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>
            {
                { "client_id", "client" },
                { "client_secret", new string('x', _options.InputLengthRestrictions.ClientSecret + 1) }
            });

            // Act
            var secret = await _parser.ParseAsync(context);

            // Assert
            secret.Should().BeNull();
        }

        [Fact]
        public async Task Missing_Client_Id_Should_Return_Null()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Request.ContentType = "application/x-www-form-urlencoded";
            context.Request.Form = new FormCollection(new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>
            {
                { "client_secret", "secret" }
            });

            // Act
            var secret = await _parser.ParseAsync(context);

            // Assert
            secret.Should().BeNull();
        }
    }
}
