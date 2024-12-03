using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer4.Models;
using Xunit;

namespace IdentityServer.UnitTests.Common
{
    public class MockPersistedGrantServiceTests
    {
        private MockPersistedGrantService _sut;

        public MockPersistedGrantServiceTests()
        {
            _sut = new MockPersistedGrantService();
        }

        [Fact]
        public async Task GetAllGrantsAsync_WhenNoGrantsSet_ShouldReturnEmpty()
        {
            var result = await _sut.GetAllGrantsAsync("subject1");
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task GetAllGrantsAsync_WhenGrantsSet_ShouldReturnConfiguredGrants()
        {
            // Arrange
            var expectedGrants = new List<Grant> 
            { 
                new Grant { ClientId = "client1" },
                new Grant { ClientId = "client2" }
            };
            _sut.GetAllGrantsResult = expectedGrants;

            // Act
            var result = await _sut.GetAllGrantsAsync("subject1");

            // Assert
            result.Should().BeEquivalentTo(expectedGrants);
        }

        [Fact]
        public async Task RemoveAllGrantsAsync_WhenCalled_ShouldSetFlag()
        {
            // Act
            await _sut.RemoveAllGrantsAsync("subject1", "client1", "session1");

            // Assert
            _sut.RemoveAllGrantsWasCalled.Should().BeTrue();
        }
    }
}
