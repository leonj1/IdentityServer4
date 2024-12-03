using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Http;
using Xunit;
using Moq;

namespace IdentityServer.UnitTests.Validation
{
    public class SecretParserTests
    {
        private class TestSecretParser : ISecretParser
        {
            public string AuthenticationMethod => "Test";

            public Task<ParsedSecret> ParseAsync(HttpContext context)
            {
                return Task.FromResult(new ParsedSecret
                {
                    Id = "test_id",
                    Credential = "test_credential",
                    Type = IdentityServerConstants.ParsedSecretTypes.SharedSecret
                });
            }
        }

        [Fact]
        public async Task ParseAsync_ShouldReturnParsedSecret()
        {
            // Arrange
            var parser = new TestSecretParser();
            var context = new DefaultHttpContext();

            // Act
            var result = await parser.ParseAsync(context);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be("test_id");
            result.Credential.Should().Be("test_credential");
            result.Type.Should().Be(IdentityServerConstants.ParsedSecretTypes.SharedSecret);
        }

        [Fact]
        public void AuthenticationMethod_ShouldReturnExpectedValue()
        {
            // Arrange
            var parser = new TestSecretParser();

            // Act & Assert
            parser.AuthenticationMethod.Should().Be("Test");
        }

        [Fact]
        public async Task ParseAsync_WithMock_ShouldCallExpectedMethod()
        {
            // Arrange
            var mockParser = new Mock<ISecretParser>();
            var context = new DefaultHttpContext();
            var expectedSecret = new ParsedSecret();
            
            mockParser.Setup(x => x.ParseAsync(context))
                     .ReturnsAsync(expectedSecret);

            // Act
            var result = await mockParser.Object.ParseAsync(context);

            // Assert
            result.Should().Be(expectedSecret);
            mockParser.Verify(x => x.ParseAsync(context), Times.Once);
        }
    }
}
