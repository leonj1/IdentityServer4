using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Xunit;
using IdentityModel;
using Newtonsoft.Json;

namespace IdentityServerHost.Quickstart.UI.Tests
{
    public class DiagnosticsViewModelTests
    {
        [Fact]
        public void Constructor_WithValidClientList_ShouldDeserializeClients()
        {
            // Arrange
            var clients = new[] { "client1", "client2" };
            var clientListJson = JsonConvert.SerializeObject(clients);
            var encodedClientList = Base64Url.Encode(Encoding.UTF8.GetBytes(clientListJson));
            
            var properties = new AuthenticationProperties();
            properties.Items.Add("client_list", encodedClientList);
            
            var authResult = AuthenticateResult.Success(
                new AuthenticationTicket(null, properties, "TestScheme"));

            // Act
            var viewModel = new DiagnosticsViewModel(authResult);

            // Assert
            Assert.NotNull(viewModel.Clients);
            Assert.Equal(clients, viewModel.Clients);
        }

        [Fact]
        public void Constructor_WithoutClientList_ShouldReturnEmptyClientsList()
        {
            // Arrange
            var properties = new AuthenticationProperties();
            var authResult = AuthenticateResult.Success(
                new AuthenticationTicket(null, properties, "TestScheme"));

            // Act
            var viewModel = new DiagnosticsViewModel(authResult);

            // Assert
            Assert.NotNull(viewModel.Clients);
            Assert.Empty(viewModel.Clients);
        }

        [Fact]
        public void AuthenticateResult_ShouldBeAccessible()
        {
            // Arrange
            var properties = new AuthenticationProperties();
            var authResult = AuthenticateResult.Success(
                new AuthenticationTicket(null, properties, "TestScheme"));

            // Act
            var viewModel = new DiagnosticsViewModel(authResult);

            // Assert
            Assert.Same(authResult, viewModel.AuthenticateResult);
        }
    }
}
