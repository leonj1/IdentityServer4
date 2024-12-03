using System;
using System.Collections.Generic;
using Xunit;

namespace IdentityServerHost.Quickstart.UI.Tests
{
    public class GrantsViewModelTests
    {
        [Fact]
        public void GrantsViewModel_Properties_ShouldWorkCorrectly()
        {
            // Arrange
            var grants = new List<GrantViewModel>
            {
                new GrantViewModel
                {
                    ClientId = "test-client",
                    ClientName = "Test Client",
                    ClientUrl = "https://test.com",
                    ClientLogoUrl = "https://test.com/logo.png",
                    Description = "Test Description",
                    Created = new DateTime(2024, 1, 1),
                    Expires = new DateTime(2024, 12, 31),
                    IdentityGrantNames = new[] { "openid", "profile" },
                    ApiGrantNames = new[] { "api1", "api2" }
                }
            };

            // Act
            var viewModel = new GrantsViewModel
            {
                Grants = grants
            };

            // Assert
            Assert.NotNull(viewModel.Grants);
            var grant = Assert.Single(viewModel.Grants);
            Assert.Equal("test-client", grant.ClientId);
            Assert.Equal("Test Client", grant.ClientName);
            Assert.Equal("https://test.com", grant.ClientUrl);
            Assert.Equal("https://test.com/logo.png", grant.ClientLogoUrl);
            Assert.Equal("Test Description", grant.Description);
            Assert.Equal(new DateTime(2024, 1, 1), grant.Created);
            Assert.Equal(new DateTime(2024, 12, 31), grant.Expires);
            Assert.Equal(new[] { "openid", "profile" }, grant.IdentityGrantNames);
            Assert.Equal(new[] { "api1", "api2" }, grant.ApiGrantNames);
        }

        [Fact]
        public void GrantViewModel_Properties_ShouldWorkCorrectly()
        {
            // Arrange & Act
            var viewModel = new GrantViewModel
            {
                ClientId = "test-client",
                ClientName = "Test Client",
                ClientUrl = "https://test.com",
                ClientLogoUrl = "https://test.com/logo.png",
                Description = "Test Description",
                Created = new DateTime(2024, 1, 1),
                Expires = new DateTime(2024, 12, 31),
                IdentityGrantNames = new[] { "openid", "profile" },
                ApiGrantNames = new[] { "api1", "api2" }
            };

            // Assert
            Assert.Equal("test-client", viewModel.ClientId);
            Assert.Equal("Test Client", viewModel.ClientName);
            Assert.Equal("https://test.com", viewModel.ClientUrl);
            Assert.Equal("https://test.com/logo.png", viewModel.ClientLogoUrl);
            Assert.Equal("Test Description", viewModel.Description);
            Assert.Equal(new DateTime(2024, 1, 1), viewModel.Created);
            Assert.Equal(new DateTime(2024, 12, 31), viewModel.Expires);
            Assert.Equal(new[] { "openid", "profile" }, viewModel.IdentityGrantNames);
            Assert.Equal(new[] { "api1", "api2" }, viewModel.ApiGrantNames);
        }

        [Fact]
        public void GrantViewModel_Expires_CanBeNull()
        {
            // Arrange & Act
            var viewModel = new GrantViewModel
            {
                Expires = null
            };

            // Assert
            Assert.Null(viewModel.Expires);
        }
    }
}
