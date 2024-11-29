using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using Xunit;

namespace IdentityServer.IntegrationTests.Clients.Setup
{
    public class DynamicParameterExtensionGrantValidatorTests
    {
        private readonly DynamicParameterExtensionGrantValidator _validator;
        private readonly ExtensionGrantValidationContext _context;

        public DynamicParameterExtensionGrantValidatorTests()
        {
            _validator = new DynamicParameterExtensionGrantValidator();
            _context = new ExtensionGrantValidationContext 
            {
                Request = new ValidatedTokenRequest
                {
                    Raw = new NameValueCollection(),
                    ClientClaims = new List<Claim>()
                }
            };
        }

        [Fact]
        public async Task When_ImpersonatedClient_Should_UpdateClientId()
        {
            // Arrange
            _context.Request.Raw.Add("impersonated_client", "new_client");

            // Act
            await _validator.ValidateAsync(_context);

            // Assert
            _context.Request.ClientId.Should().Be("new_client");
        }

        [Fact]
        public async Task When_Lifetime_Should_UpdateAccessTokenLifetime()
        {
            // Arrange
            _context.Request.Raw.Add("lifetime", "3600");

            // Act
            await _validator.ValidateAsync(_context);

            // Assert
            _context.Request.AccessTokenLifetime.Should().Be(3600);
        }

        [Theory]
        [InlineData("jwt", AccessTokenType.Jwt)]
        [InlineData("reference", AccessTokenType.Reference)]
        public async Task When_TokenType_Should_UpdateAccessTokenType(string type, AccessTokenType expected)
        {
            // Arrange
            _context.Request.Raw.Add("type", type);

            // Act
            await _validator.ValidateAsync(_context);

            // Assert
            _context.Request.AccessTokenType.Should().Be(expected);
        }

        [Fact]
        public async Task When_ExtraClaim_Should_AddToClientClaims()
        {
            // Arrange
            _context.Request.Raw.Add("claim", "test_claim");

            // Act
            await _validator.ValidateAsync(_context);

            // Assert
            var claim = _context.Request.ClientClaims.Single();
            claim.Type.Should().Be("extra");
            claim.Value.Should().Be("test_claim");
        }

        [Fact]
        public async Task When_Sub_Should_ReturnDelegationResult()
        {
            // Arrange
            _context.Request.Raw.Add("sub", "test_subject");

            // Act
            await _validator.ValidateAsync(_context);

            // Assert
            _context.Result.Subject.Should().Be("test_subject");
            _context.Result.GrantType.Should().Be("delegation");
        }

        [Fact]
        public async Task When_NoSub_Should_ReturnDefaultResult()
        {
            // Act
            await _validator.ValidateAsync(_context);

            // Assert
            _context.Result.Should().NotBeNull();
            _context.Result.Subject.Should().BeNull();
        }

        [Fact]
        public void GrantType_Should_ReturnDynamic()
        {
            // Act & Assert
            _validator.GrantType.Should().Be("dynamic");
        }
    }
}
