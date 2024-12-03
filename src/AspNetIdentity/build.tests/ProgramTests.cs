using Xunit;

namespace build.tests
{
    public class ProgramTests
    {
        [Fact]
        public void Prefix_ShouldBeAspNetIdentity()
        {
            // Access the constant through reflection since it's private
            var field = typeof(Program).GetField("Prefix", 
                System.Reflection.BindingFlags.NonPublic | 
                System.Reflection.BindingFlags.Static);
            
            var value = field.GetValue(null) as string;
            
            Assert.Equal("AspNetIdentity", value);
        }
    }
}
