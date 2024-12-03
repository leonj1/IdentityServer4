using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Moq;

namespace SqlServer.Tests
{
    public class ProgramTests
    {
        [Fact]
        public void BuildWebHost_ReturnsValidWebHost()
        {
            // Arrange
            string[] args = new string[] { };

            // Act
            var webHost = Program.BuildWebHost(args);

            // Assert
            Assert.NotNull(webHost);
        }

        [Fact]
        public void Main_CallsSeedData()
        {
            // Arrange
            string[] args = new string[] { };
            
            // Act & Assert - Should not throw any exceptions
            Program.Main(args);
        }
    }
}
