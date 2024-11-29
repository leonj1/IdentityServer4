using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer4.Validation;
using IdentityServerHost.Extensions;
using Xunit;

namespace IdentityServer.UnitTests.Extensions
{
    public class ParameterizedScopeTokenRequestValidatorTests
    {
        private readonly ParameterizedScopeTokenRequestValidator _validator;

        public ParameterizedScopeTokenRequestValidatorTests()
        {
            _validator = new ParameterizedScopeTokenRequestValidator();
        }

        [Fact]
        public async Task When_Transaction_Scope_Present_Should_Add_Claim()
        {
            // Arrange
            var context = new CustomTokenRequestValidationContext
            {
                Result = new TokenRequestValidationResult(new ValidatedTokenRequest())
            };
            
            context.Result.ValidatedRequest.ValidatedResources = new ResourceValidationResult();
            context.Result.ValidatedRequest.ValidatedResources.ParsedScopes = new[]
            {
                new ParsedScopeValue("transaction", "123")
            };

            // Act
            await _validator.ValidateAsync(context);

            // Assert
            var claim = context.Result.ValidatedRequest.ClientClaims.Single();
            claim.Type.Should().Be("transaction");
            claim.Value.Should().Be("123");
        }

        [Fact]
        public async Task When_No_Transaction_Scope_Should_Not_Add_Claim()
        {
            // Arrange
            var context = new CustomTokenRequestValidationContext
            {
                Result = new TokenRequestValidationResult(new ValidatedTokenRequest())
            };
            
            context.Result.ValidatedRequest.ValidatedResources = new ResourceValidationResult();
            context.Result.ValidatedRequest.ValidatedResources.ParsedScopes = new[]
            {
                new ParsedScopeValue("other_scope", null)
            };

            // Act
            await _validator.ValidateAsync(context);

            // Assert
            context.Result.ValidatedRequest.ClientClaims.Should().BeEmpty();
        }
    }
}
