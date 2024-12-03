using Xunit;

namespace build.UnitTests
{
    public class ProgramTests
    {
        [Fact]
        public void Prefix_ShouldBeStorage()
        {
            // Access the constant through reflection since it's private
            var field = typeof(Program).GetField("Prefix", 
                System.Reflection.BindingFlags.NonPublic | 
                System.Reflection.BindingFlags.Static);
            
            var value = field.GetValue(null) as string;
            
            Assert.Equal("Storage", value);
        }
    }
}
