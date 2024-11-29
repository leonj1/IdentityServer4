using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using IdentityServer4.Validation;
using Xunit;

namespace IdentityServer.UnitTests.Validation.Setup
{
    public class FactoryTests
    {
        [Fact]
        public void CreateClientStore_ShouldReturnValidStore()
        {
            // Act
            var store = Factory.CreateClientStore();

            // Assert
            store.Should().NotBeNull();
            store.Should().BeAssignableFrom<IClientStore>();
        }

        [Fact]
        public void CreateTokenRequestValidator_WithDefaultParameters_ShouldReturnValidator()
        {
            // Act
            var validator = Factory.CreateTokenRequestValidator();

            // Assert
            validator.Should().NotBeNull();
        }

        [Fact]
        public void CreateTokenValidator_WithDefaultParameters_ShouldReturnValidator()
        {
            // Act
            var validator = Factory.CreateTokenValidator();

            // Assert
            validator.Should().NotBeNull();
        }

        [Fact]
        public void CreateAuthorizeRequestValidator_WithDefaultParameters_ShouldReturnValidator()
        {
            // Act
            var validator = Factory.CreateAuthorizeRequestValidator();

            // Assert
            validator.Should().NotBeNull();
        }

        [Fact]
        public void CreateDeviceAuthorizationRequestValidator_WithDefaultParameters_ShouldReturnValidator()
        {
            // Act
            var validator = Factory.CreateDeviceAuthorizationRequestValidator();

            // Assert
            validator.Should().NotBeNull();
        }

        [Fact]
        public void CreateClientSecretValidator_WithDefaultParameters_ShouldReturnValidator()
        {
            // Act
            var validator = Factory.CreateClientSecretValidator();

            // Assert
            validator.Should().NotBeNull();
        }

        [Fact]
        public void CreateRefreshTokenStore_ShouldReturnStore()
        {
            // Act
            var store = Factory.CreateRefreshTokenStore();

            // Assert
            store.Should().NotBeNull();
            store.Should().BeAssignableFrom<IRefreshTokenStore>();
        }

        [Fact]
        public void CreateReferenceTokenStore_ShouldReturnStore()
        {
            // Act
            var store = Factory.CreateReferenceTokenStore();

            // Assert
            store.Should().NotBeNull();
            store.Should().BeAssignableFrom<IReferenceTokenStore>();
        }

        [Fact]
        public void CreateDeviceCodeService_ShouldReturnService()
        {
            // Act
            var service = Factory.CreateDeviceCodeService();

            // Assert
            service.Should().NotBeNull();
            service.Should().BeAssignableFrom<IDeviceFlowCodeService>();
        }

        [Fact]
        public void CreateUserConsentStore_ShouldReturnStore()
        {
            // Act
            var store = Factory.CreateUserConsentStore();

            // Assert
            store.Should().NotBeNull();
            store.Should().BeAssignableFrom<IUserConsentStore>();
        }
    }
}
