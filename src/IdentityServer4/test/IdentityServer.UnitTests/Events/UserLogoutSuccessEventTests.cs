using IdentityServer4.Events;
using Xunit;

namespace IdentityServer.UnitTests.Events
{
    public class UserLogoutSuccessEventTests
    {
        [Fact]
        public void Constructor_ShouldSetCorrectProperties()
        {
            // Arrange
            var subjectId = "test_subject";
            var name = "test_name";

            // Act
            var evt = new UserLogoutSuccessEvent(subjectId, name);

            // Assert
            Assert.Equal(EventCategories.Authentication, evt.Category);
            Assert.Equal("User Logout Success", evt.Name);
            Assert.Equal(EventTypes.Success, evt.EventType);
            Assert.Equal(EventIds.UserLogoutSuccess, evt.Id);
            Assert.Equal(subjectId, evt.SubjectId);
            Assert.Equal(name, evt.DisplayName);
        }

        [Theory]
        [InlineData(null, "name")]
        [InlineData("subject", null)]
        [InlineData(null, null)]
        public void Constructor_ShouldHandleNullValues(string subjectId, string name)
        {
            // Act
            var evt = new UserLogoutSuccessEvent(subjectId, name);

            // Assert
            Assert.Equal(subjectId, evt.SubjectId);
            Assert.Equal(name, evt.DisplayName);
        }
    }
}
