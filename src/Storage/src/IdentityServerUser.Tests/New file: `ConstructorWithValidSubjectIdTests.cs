using System;
using Xunit;

namespace IdentityServer4.Tests
{
    public class ConstructorWithValidSubjectIdTests
    {
        [Fact]
        public void Constructor_WithValidSubjectId_CreatesInstance()
        {
            // Arrange & Act
            var user = new IdentityServerUser("123");

            // Assert
            Assert.Equal("123", user.SubjectId);
        }
    }
}
