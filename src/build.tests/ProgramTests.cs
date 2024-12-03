using System;
using System.IO;
using Xunit;
using build;

namespace build.tests
{
    public class ProgramTests
    {
        [Fact]
        public void Sign_ThrowsException_WhenSignClientSecretMissing()
        {
            // Arrange
            Environment.SetEnvironmentVariable("SignClientSecret", null);

            // Act & Assert
            var exception = Assert.Throws<Exception>(() => 
                Program.SignForTest("./testPath", "*.dll"));
            Assert.Equal("SignClientSecret environment variable is missing. Aborting.", exception.Message);
        }

        [Fact]
        public void CleanPackOutput_DeletesDirectory_WhenExists()
        {
            // Arrange
            var testDir = Path.Combine(Path.GetTempPath(), "packOutput");
            Directory.CreateDirectory(testDir);
            File.WriteAllText(Path.Combine(testDir, "test.txt"), "test");

            // Act
            Program.CleanPackOutputForTest(testDir);

            // Assert
            Assert.False(Directory.Exists(testDir));
        }

        [Fact]
        public void CleanPackOutput_DoesNotThrow_WhenDirectoryDoesNotExist()
        {
            // Arrange
            var testDir = Path.Combine(Path.GetTempPath(), "nonexistentDir");

            // Act & Assert
            var exception = Record.Exception(() => Program.CleanPackOutputForTest(testDir));
            Assert.Null(exception);
        }
    }
}
