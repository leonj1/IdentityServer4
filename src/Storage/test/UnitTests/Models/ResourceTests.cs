using System.Collections.Generic;
using FluentAssertions;
using IdentityServer4.Models;
using Xunit;

namespace IdentityServer4.UnitTests.Models
{
    public class ResourceTests
    {
        private class TestResource : Resource { }

        [Fact]
        public void DefaultConstructor_ShouldSetDefaultValues()
        {
            var resource = new TestResource();

            resource.Enabled.Should().BeTrue();
            resource.ShowInDiscoveryDocument.Should().BeTrue();
            resource.UserClaims.Should().BeEmpty();
            resource.Properties.Should().BeEmpty();
        }

        [Fact]
        public void Setting_Properties_ShouldWork()
        {
            var resource = new TestResource
            {
                Name = "test-resource",
                DisplayName = "Test Resource",
                Description = "A test resource",
                Enabled = false,
                ShowInDiscoveryDocument = false
            };

            resource.Name.Should().Be("test-resource");
            resource.DisplayName.Should().Be("Test Resource");
            resource.Description.Should().Be("A test resource");
            resource.Enabled.Should().BeFalse();
            resource.ShowInDiscoveryDocument.Should().BeFalse();
        }

        [Fact]
        public void AddingUserClaims_ShouldWork()
        {
            var resource = new TestResource();
            resource.UserClaims.Add("claim1");
            resource.UserClaims.Add("claim2");

            resource.UserClaims.Should().HaveCount(2);
            resource.UserClaims.Should().Contain(new[] { "claim1", "claim2" });
        }

        [Fact]
        public void AddingProperties_ShouldWork()
        {
            var resource = new TestResource();
            resource.Properties.Add("key1", "value1");
            resource.Properties.Add("key2", "value2");

            resource.Properties.Should().HaveCount(2);
            resource.Properties["key1"].Should().Be("value1");
            resource.Properties["key2"].Should().Be("value2");
        }
    }
}
