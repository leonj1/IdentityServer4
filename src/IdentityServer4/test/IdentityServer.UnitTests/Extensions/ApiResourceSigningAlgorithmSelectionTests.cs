using FluentAssertions;
using Xunit;

namespace YourNamespace.Tests // Replace with your actual namespace
{
    public class ApiResourceTests
    {
        [Fact]
        public void Single_resource_no_allowed_algorithms_set_should_return_empty_list()
        {
            var resource = new ApiResource();

            var allowedAlgorithms = resource.FindMatchingSigningAlgorithms();

            allowedAlgorithms.Should().BeEmpty();
        }
    }
}
