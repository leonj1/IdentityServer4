using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Xunit;
using FluentAssertions;

namespace IdentityServer.UnitTests.Services
{
    public class PersistedGrantServiceTests
    {
        private readonly MockPersistedGrantService _service;

        public PersistedGrantServiceTests()
        {
            _service = new MockPersistedGrantService();
        }

        [Fact]
        public async Task GetAllGrantsAsync_WhenSubjectIdExists_ShouldReturnGrants()
        {
            // Arrange
            var subjectId = "test_subject";
            var expectedGrants = new List<Grant>
            {
                new Grant { ClientId = "client1", SubjectId = subjectId },
                new Grant { ClientId = "client2", SubjectId = subjectId }
            };

            // Act
            var result = await _service.GetAllGrantsAsync(subjectId);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedGrants);
        }

        [Fact]
        public async Task GetAllGrantsAsync_WhenSubjectIdDoesNotExist_ShouldReturnEmptyCollection()
        {
            // Arrange
            var subjectId = "non_existent_subject";

            // Act
            var result = await _service.GetAllGrantsAsync(subjectId);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task RemoveAllGrantsAsync_ShouldRemoveGrantsForSubject()
        {
            // Arrange
            var subjectId = "test_subject";
            var clientId = "test_client";

            // Act
            await _service.RemoveAllGrantsAsync(subjectId, clientId);
            var remainingGrants = await _service.GetAllGrantsAsync(subjectId);

            // Assert
            remainingGrants.Should().NotBeNull();
            remainingGrants.Should().BeEmpty();
        }

        [Fact]
        public async Task RemoveAllGrantsAsync_WithSessionId_ShouldRemoveSpecificGrants()
        {
            // Arrange
            var subjectId = "test_subject";
            var clientId = "test_client";
            var sessionId = "test_session";

            // Act
            await _service.RemoveAllGrantsAsync(subjectId, clientId, sessionId);
            var remainingGrants = await _service.GetAllGrantsAsync(subjectId);

            // Assert
            remainingGrants.Should().NotBeNull();
            remainingGrants.Where(g => g.SessionId == sessionId).Should().BeEmpty();
        }
    }
}
