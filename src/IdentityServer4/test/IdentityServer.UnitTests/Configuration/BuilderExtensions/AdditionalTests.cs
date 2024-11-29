using FluentAssertions;
using IdentityServer4.ResponseHandling;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using IdentityServer4.Validation;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace IdentityServer.UnitTests.Configuration.BuilderExtensions
{
    public class AdditionalTests
    {
        private readonly IServiceCollection _services;
        private readonly IIdentityServerBuilder _builder;

        public AdditionalTests()
        {
            _services = new ServiceCollection();
            _builder = new IdentityServerBuilder(_services);
        }

        [Fact]
        public void AddExtensionGrantValidator_Should_RegisterValidator()
        {
            _builder.AddExtensionGrantValidator<TestExtensionGrantValidator>();

            var descriptor = _services.Should().Contain(x => 
                x.ServiceType == typeof(IExtensionGrantValidator) && 
                x.ImplementationType == typeof(TestExtensionGrantValidator))
                .Subject;
            
            descriptor.Lifetime.Should().Be(ServiceLifetime.Transient);
        }

        [Fact]
        public void AddResourceOwnerValidator_Should_RegisterValidator()
        {
            _builder.AddResourceOwnerValidator<TestResourceOwnerPasswordValidator>();

            var descriptor = _services.Should().Contain(x =>
                x.ServiceType == typeof(IResourceOwnerPasswordValidator) &&
                x.ImplementationType == typeof(TestResourceOwnerPasswordValidator))
                .Subject;

            descriptor.Lifetime.Should().Be(ServiceLifetime.Transient);
        }

        [Fact]
        public void AddProfileService_Should_RegisterService()
        {
            _builder.AddProfileService<TestProfileService>();

            var descriptor = _services.Should().Contain(x =>
                x.ServiceType == typeof(IProfileService) &&
                x.ImplementationType == typeof(TestProfileService))
                .Subject;

            descriptor.Lifetime.Should().Be(ServiceLifetime.Transient);
        }

        private class TestExtensionGrantValidator : IExtensionGrantValidator
        {
            public string GrantType => "test";
            public Task ValidateAsync(ExtensionGrantValidationContext context) => Task.CompletedTask;
        }

        private class TestResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
        {
            public Task ValidateAsync(ResourceOwnerPasswordValidationContext context) => Task.CompletedTask;
        }

        private class TestProfileService : IProfileService
        {
            public Task GetProfileDataAsync(ProfileDataRequestContext context) => Task.CompletedTask;
            public Task IsActiveAsync(IsActiveContext context) => Task.CompletedTask;
        }
    }
}
