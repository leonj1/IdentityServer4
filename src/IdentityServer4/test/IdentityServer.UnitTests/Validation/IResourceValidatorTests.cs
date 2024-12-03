using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using Xunit;

namespace IdentityServer.UnitTests.Validation
{
    public class IResourceValidatorTests
    {
        private class TestResourceValidator : IResourceValidator
        {
            private readonly ResourceValidationResult _result;

            public TestResourceValidator(ResourceValidationResult result)
            {
                _result = result;
            }

            public Task<ResourceValidationResult> ValidateRequestedResourcesAsync(ResourceValidationRequest request)
            {
                return Task.FromResult(_result);
            }
        }

        [Fact]
        public async Task ValidateRequestedResourcesAsync_ShouldReturnValidResult()
        {
            // Arrange
            var expectedResources = new Resources
            {
                IdentityResources = new List<IdentityResource> { new IdentityResource("openid", "OpenID") },
                ApiResources = new List<ApiResource> { new ApiResource("api1", "API 1") },
                ApiScopes = new List<ApiScope> { new ApiScope("scope1", "Scope 1") }
            };

            var validator = new TestResourceValidator(new ResourceValidationResult(expectedResources));
            var request = new ResourceValidationRequest();

            // Act
            var result = await validator.ValidateRequestedResourcesAsync(request);

            // Assert
            result.Should().NotBeNull();
            result.Resources.Should().NotBeNull();
            result.Resources.IdentityResources.Should().HaveCount(1);
            result.Resources.ApiResources.Should().HaveCount(1);
            result.Resources.ApiScopes.Should().HaveCount(1);
        }

        [Fact]
        public async Task ValidateRequestedResourcesAsync_ShouldHandleNullResources()
        {
            // Arrange
            var validator = new TestResourceValidator(new ResourceValidationResult(null));
            var request = new ResourceValidationRequest();

            // Act
            var result = await validator.ValidateRequestedResourcesAsync(request);

            // Assert
            result.Should().NotBeNull();
            result.Resources.Should().BeNull();
        }

        [Fact]
        public async Task ValidateRequestedResourcesAsync_ShouldHandleEmptyResources()
        {
            // Arrange
            var validator = new TestResourceValidator(new ResourceValidationResult(new Resources()));
            var request = new ResourceValidationRequest();

            // Act
            var result = await validator.ValidateRequestedResourcesAsync(request);

            // Assert
            result.Should().NotBeNull();
            result.Resources.Should().NotBeNull();
            result.Resources.IdentityResources.Should().BeEmpty();
            result.Resources.ApiResources.Should().BeEmpty();
            result.Resources.ApiScopes.Should().BeEmpty();
        }
    }
}
