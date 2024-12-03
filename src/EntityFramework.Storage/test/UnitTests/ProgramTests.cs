using Xunit;

namespace EntityFramework.Storage.UnitTests
{
    public class ProgramTests
    {
        [Fact]
        public void Prefix_ShouldHaveCorrectValue()
        {
            // Arrange & Act
            var prefix = build.Program.Prefix;

            // Assert
            Assert.Equal("EntityFramework.Storage", prefix);
        }
    }
}
