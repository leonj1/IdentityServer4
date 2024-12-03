using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using IdentityServerHost.Quickstart.UI;

namespace IdentityServerHost.Quickstart.Tests
{
    public class GrantsViewModelTests
    {
        [Fact]
        public void GrantsViewModel_Properties_ShouldInitializeCorrectly()
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
                    Created = DateTime.Now,
                    Expires = DateTime.Now.AddDays(1),
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
            var grant = viewModel.Grants.First();
            Assert.Equal("test-client", grant.ClientId);
            Assert.Equal("Test Client", grant.ClientName);
            Assert.Equal("https://test.com", grant.ClientUrl);
            Assert.Equal("https://test.com/logo.png", grant.ClientLogoUrl);
            Assert.Equal("Test Description", grant.Description);
            Assert.NotNull(grant.Created);
            Assert.NotNull(grant.Expires);
            Assert.Equal(2, grant.IdentityGrantNames.Count());
            Assert.Equal(2, grant.ApiGrantNames.Count());
        }

        [Fact]
        public void GrantViewModel_Properties_ShouldHandleNullValues()
        {
            // Arrange & Act
            var grant = new GrantViewModel();

            // Assert
            Assert.Null(grant.ClientId);
            Assert.Null(grant.ClientName);
            Assert.Null(grant.ClientUrl);
            Assert.Null(grant.ClientLogoUrl);
            Assert.Null(grant.Description);
            Assert.Equal(default(DateTime), grant.Created);
            Assert.Null(grant.Expires);
            Assert.Null(grant.IdentityGrantNames);
            Assert.Null(grant.ApiGrantNames);
        }
    }
}
