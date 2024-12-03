using FluentAssertions;
using IdentityServer4.Configuration;
using IdentityServer4.Services;
using IdentityServer4.Services.Default;
using IdentityServer4.Stores;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace IdentityServer.UnitTests.Configuration
{
    public class IdentityServerBuilderExtensionsCoreTests
    {
        private IServiceCollection _services;
        private IIdentityServerBuilder _builder;

        public IdentityServerBuilderExtensionsCoreTests()
        {
            _services = new ServiceCollection();
            _builder = new IdentityServerBuilder(_services);
        }

        [Fact]
        public void AddRequiredPlatformServices_RegistersCorrectServices()
        {
            _builder.AddRequiredPlatformServices();

            // Verify core services are registered
            _services.Should().Contain(x => x.ServiceType == typeof(IHttpContextAccessor));
            _services.Should().Contain(x => x.ServiceType == typeof(IdentityServerOptions));
            _services.Should().Contain(x => x.ServiceType == typeof(IHttpClientFactory));
        }

        [Fact]
        public void AddCoreServices_RegistersCorrectServices()
        {
            _builder.AddCoreServices();

            // Verify essential services
            _services.Should().Contain(x => x.ServiceType == typeof(ISecretsListParser));
            _services.Should().Contain(x => x.ServiceType == typeof(ISecretsListValidator));
            _services.Should().Contain(x => x.ServiceType == typeof(IReturnUrlParser));
            _services.Should().Contain(x => x.ServiceType == typeof(IUserSession));
        }

        [Fact]
        public void AddPluggableServices_RegistersCorrectServices()
        {
            _builder.AddPluggableServices();

            // Verify key services
            _services.Should().Contain(x => x.ServiceType == typeof(IPersistedGrantService));
            _services.Should().Contain(x => x.ServiceType == typeof(ITokenService));
            _services.Should().Contain(x => x.ServiceType == typeof(IConsentService));
            _services.Should().Contain(x => x.ServiceType == typeof(IProfileService));
        }

        [Fact]
        public void AddValidators_RegistersCorrectServices()
        {
            _builder.AddValidators();

            // Verify validators
            _services.Should().Contain(x => x.ServiceType == typeof(ITokenValidator));
            _services.Should().Contain(x => x.ServiceType == typeof(IAuthorizeRequestValidator));
            _services.Should().Contain(x => x.ServiceType == typeof(ITokenRequestValidator));
            _services.Should().Contain(x => x.ServiceType == typeof(IRedirectUriValidator));
        }

        [Fact]
        public void AddDefaultSecretParsers_RegistersCorrectParsers()
        {
            _builder.AddDefaultSecretParsers();

            _services.Should().Contain(x => x.ServiceType == typeof(ISecretParser));
        }

        [Fact]
        public void AddDefaultSecretValidators_RegistersCorrectValidators()
        {
            _builder.AddDefaultSecretValidators();

            _services.Should().Contain(x => x.ServiceType == typeof(ISecretValidator));
        }
    }
}
