using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using IdentityServer4.Models;

namespace IdentityServerHost.Quickstart.UI.Tests
{
    public class GrantsControllerTests
    {
        private readonly Mock<IIdentityServerInteractionService> _interactionMock;
        private readonly Mock<IClientStore> _clientStoreMock;
        private readonly Mock<IResourceStore> _resourceStoreMock;
        private readonly Mock<IEventService> _eventServiceMock;
        private readonly GrantsController _controller;

        public GrantsControllerTests()
        {
            _interactionMock = new Mock<IIdentityServerInteractionService>();
            _clientStoreMock = new Mock<IClientStore>();
            _resourceStoreMock = new Mock<IResourceStore>();
            _eventServiceMock = new Mock<IEventService>();

            _controller = new GrantsController(
                _interactionMock.Object,
                _clientStoreMock.Object,
                _resourceStoreMock.Object,
                _eventServiceMock.Object
            );
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
                    Scopes = new[] { "scope1", "scope2" }
                }
            };

            var client = new Client
            {
                ClientId = "client1",
                ClientName = "Test Client"
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

            _interactionMock.Setup(x => x.GetAllUserGrantsAsync())
                .ReturnsAsync(grants);
            _clientStoreMock.Setup(x => x.FindClientByIdAsync("client1"))
                .ReturnsAsync(client);
            _resourceStoreMock.Setup(x => x.FindResourcesByScopeAsync(It.IsAny<IEnumerable<string>>()))
                .ReturnsAsync(resources);

            // Act
            var result = await _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<GrantsViewModel>(viewResult.Model);
            Assert.Single(model.Grants);
            Assert.Equal("client1", model.Grants[0].ClientId);
            Assert.Equal("Test Client", model.Grants[0].ClientName);
        }

        [Fact]
        public async Task Revoke_RevokesGrantAndRedirectsToIndex()
        {
            // Arrange
            var clientId = "client1";

            // Act
            var result = await _controller.Revoke(clientId);

            // Assert
            _interactionMock.Verify(x => x.RevokeUserConsentAsync(clientId), Times.Once);
            _eventServiceMock.Verify(x => x.RaiseAsync(It.IsAny<IdentityServer4.Events.GrantsRevokedEvent>()), Times.Once);
            
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
        }
    }
}
