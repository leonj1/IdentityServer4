using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityServer4.Validation;
using Xunit;

namespace IdentityServer.UnitTests.Common
{
    public class MockResourceValidatorTests
    {
        private MockResourceValidator _validator;

        public MockResourceValidatorTests()
        {
            _validator = new MockResourceValidator();
        }

        [Fact]
        public async Task ParseRequestedScopesAsync_ShouldReturnParsedScopeValues()
        {
            // Arrange
            var scopeValues = new[] { "scope1", "scope2" };

            // Act
            var result = await _validator.ParseRequestedScopesAsync(scopeValues);

            // Assert
            Assert.NotNull(result);
            var parsedScopes = result.ToList();
            Assert.Equal(2, parsedScopes.Count);
            Assert.Equal("scope1", parsedScopes[0].RawValue);
            Assert.Equal("scope2", parsedScopes[1].RawValue);
        }

        [Fact]
        public async Task ValidateRequestedResourcesAsync_ShouldReturnConfiguredResult()
        {
            // Arrange
            var expectedResult = new ResourceValidationResult();
            _validator.Result = expectedResult;
            var request = new ResourceValidationRequest();

            // Act
            var result = await _validator.ValidateRequestedResourcesAsync(request);

            // Assert
            Assert.Same(expectedResult, result);
        }

        [Fact]
        public async Task ValidateRequestedResourcesAsync_WithNullRequest_ShouldStillReturnResult()
        {
            // Arrange
            var expectedResult = new ResourceValidationResult();
            _validator.Result = expectedResult;

            // Act
            var result = await _validator.ValidateRequestedResourcesAsync(null);

            // Assert
            Assert.Same(expectedResult, result);
        }
    }
}
