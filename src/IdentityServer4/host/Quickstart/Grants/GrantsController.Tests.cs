using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Events;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace IdentityServerHost.Quickstart.UI.Tests
{
    public class GrantsControllerTests
    {
        private readonly Mock<IIdentityServerInteractionService> _mockInteraction;
        private readonly Mock<IClientStore> _mockClientStore;
        private readonly Mock<IResourceStore> _mockResourceStore;
        private readonly Mock<IEventService> _mockEventService;
        private readonly GrantsController _controller;

        public GrantsControllerTests()
        {
            _mockInteraction = new Mock<IIdentityServerInteractionService>();
            _mockClientStore = new Mock<IClientStore>();
            _mockResourceStore = new Mock<IResourceStore>();
            _mockEventService = new Mock<IEventService>();

            _controller = new GrantsController(
                _mockInteraction.Object,
                _mockClientStore.Object,
                _mockResourceStore.Object,
                _mockEventService.Object);
        }

        [Fact]
        public async Task Index_ReturnsViewWithGrantsViewModel()
        {
            // Arrange
            var grants = new List<Grant>
            {
                new Grant
                {
                    ClientId = "client1",
                    Description = "Test Grant",
                    CreationTime = DateTime.UtcNow,
                    Scopes = new[] { "scope1", "scope2" }
                }
            };

            var client = new Client
            {
                ClientId = "client1",
                ClientName = "Test Client",
                LogoUri = "http://example.com/logo.png",
                ClientUri = "http://example.com"
            };

            var resources = new Resources
            {
                IdentityResources = new List<IdentityResource>
                {
                    new IdentityResource { Name = "scope1", DisplayName = "Scope 1" }
                },
                ApiScopes = new List<ApiScope>
                {
                    new ApiScope { Name = "scope2", DisplayName = "Scope 2" }
                }
            };

            _mockInteraction.Setup(x => x.GetAllUserGrantsAsync())
                .ReturnsAsync(grants);
            _mockClientStore.Setup(x => x.FindClientByIdAsync("client1"))
                .ReturnsAsync(client);
            _mockResourceStore.Setup(x => x.FindResourcesByScopeAsync(It.IsAny<IEnumerable<string>>()))
                .ReturnsAsync(resources);

            // Act
            var result = await _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<GrantsViewModel>(viewResult.Model);
            Assert.Single(model.Grants);
            var grant = model.Grants.First();
            Assert.Equal("client1", grant.ClientId);
            Assert.Equal("Test Client", grant.ClientName);
            Assert.Equal("http://example.com/logo.png", grant.ClientLogoUrl);
            Assert.Equal("http://example.com", grant.ClientUrl);
            Assert.Equal("Test Grant", grant.Description);
            Assert.Single(grant.IdentityGrantNames);
            Assert.Single(grant.ApiGrantNames);
        }

        [Fact]
        public async Task Revoke_RevokesGrantAndRaisesEvent()
        {
            // Arrange
            var clientId = "client1";

            // Act
            var result = await _controller.Revoke(clientId);

            // Assert
            _mockInteraction.Verify(x => x.RevokeUserConsentAsync(clientId), Times.Once);
            _mockEventService.Verify(x => x.RaiseAsync(It.IsAny<GrantsRevokedEvent>()), Times.Once);
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
        }
    }
}
