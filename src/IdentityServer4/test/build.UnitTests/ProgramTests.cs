using Xunit;

namespace build.UnitTests
{
    public class ProgramTests
    {
        [Fact]
        public void Prefix_ShouldHaveCorrectValue()
        {
            // Arrange & Act
            var prefix = Program.Prefix;

            // Assert
            Assert.Equal("IdentityServer4", prefix);
        }
    }
}
