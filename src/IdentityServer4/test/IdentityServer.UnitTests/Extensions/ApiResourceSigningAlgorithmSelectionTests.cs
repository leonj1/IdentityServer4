using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using IdentityServer4.Models;
using Xunit;

namespace IdentityServer.UnitTests.Extensions
{
    public class ApiResourceSigningAlgorithmSelectionTests
    {
        [Fact]
        public void Single_resource_no_allowed_algorithms_set_should_return_empty_list()
        {
            var resource = new ApiResource();

            var allowedAlgorithms = new List<ApiResource> { resource }.FindMatchingSigningAlgorithms();

            allowedAlgorithms.Count().Should().Be(0);
        }
        
        [Fact]
        public void Two_resources_no_allowed_algorithms_set_should_return_empty_list()
        {
            var resource1 = new ApiResource();
            var resource2 = new ApiResource();

            var allowedAlgorithms = new List<ApiResource> { resource1, resource2 }.FindMatchingSigningAlgorithms();

            allowedAlgorithms.Count().Should().Be(0);
        }
        
        [Theory]
        [InlineData(new [] { "A" }, new [] { "A" }, 
                    new [] { "A" })]
        [InlineData(new [] { "A", "B" }, new [] { "A", "B" }, 
                    new [] { "A", "B" })]
        [InlineData(new [] { "A", "B", "C" }, new [] { "A", "B", "C" }, 
                    new [] { "A", "B", "C" })]

        [InlineData(new [] { "A", "B" }, new [] { "A", "D" }, 
                    new [] { "A" })]
        [InlineData(new [] { "A", "B", "C" }, new [] { "A", "B", "Z" }, 
                    new [] { "A", "B" })]

        [InlineData(new string[] { }, new [] { "B" },
                    new string[] { "B" })]
        [InlineData(new string[] { }, new [] { "C", "D" },
                    new string[] { "C", "D" })]

        [InlineData(new [] { "A" }, new [] { "B" }, 
                    new string[] { })]
        [InlineData(new [] { "A", "B" }, new [] { "C", "D" }, 
                    new string[] { })]
        public void Two_resources_with_allowed_algorithms_set_should_return_right_values(
            string[] resource1Algorithms, string[] resource2Algorithms, 
            string[] expectedAlgorithms)
        {
            var resource1 = new ApiResource()
            {
                AllowedAccessTokenSigningAlgorithms = resource1Algorithms
            };
            
            var resource2 = new ApiResource
            {
                AllowedAccessTokenSigningAlgorithms = resource2Algorithms
            };

            if (expectedAlgorithms.Any())
            {
                var allowedAlgorithms = new List<ApiResource> { resource1, resource2 }.FindMatchingSigningAlgorithms();
                allowedAlgorithms.Should().BeEquivalentTo(expectedAlgorithms);
            }
            else
            {
                Action act = () => new List<ApiResource> { resource1, resource2 }.FindMatchingSigningAlgorithms();
                act.Should().Throw<InvalidOperationException>();
            }
        }
        
        [Theory]
        [InlineData(new [] { "A" }, new [] { "A" }, new [] { "A" }, 
            new [] { "A" })]
        [InlineData(new [] { "A", "B" }, new [] { "A", "B" }, new [] { "A", "B" }, 
            new [] { "A", "B" })]
        [InlineData(new [] { "A", "B", "C" }, new [] { "A", "B", "C" }, new [] { "A", "B", "C" }, 
            new [] { "A", "B", "C" })]
        
        [InlineData(new [] { "A", "B" }, new [] { "A", "D" }, new [] { "A", "E" } ,
                    new [] { "A" })]
        [InlineData(new [] { "A", "B", "X" }, new [] { "A", "B", "Y" }, new [] { "A", "B", "Z" },
                    new [] { "A", "B" })]
        [InlineData(new [] { "A", "B", "X" }, new [] { "C", "D", "X" }, new [] { "E", "F", "X" },
                    new [] { "X" })]

        [InlineData(new[] { "A", "B" }, new[] { "A", "D" }, new string[] { },
                    new[] { "A" })]
        [InlineData(new[] { "A", "B" }, new[] { "A", "C", "B" }, new string[] { },
                    new[] { "A", "B" })]
        [InlineData(new[] { "A", "B" }, new string[] { }, new string[] { },
                    new[] { "A", "B" })]

        [InlineData(new [] { "A" }, new [] { "B" }, new [] { "C" }, 
            new string[] { })]
        [InlineData(new [] { "A", "B" }, new [] { "C", "D" }, new [] { "X", "Y" }, 
            new string[] { })]
        [InlineData(new [] { "A", "B", "C" }, new [] { "C", "D", "E" }, new [] { "E", "F", "G" },
                    new string[] { })]
        public void Three_resources_with_allowed_algorithms_set_should_return_right_values(
            string[] resource1Algorithms, string[] resource2Algorithms, string[] resource3Algorithms, 
            string[] expectedAlgorithms)
        {
            var resource1 = new ApiResource()
            {
                AllowedAccessTokenSigningAlgorithms = resource1Algorithms
            };
            
            var resource2 = new ApiResource
            {
                AllowedAccessTokenSigningAlgorithms = resource2Algorithms
            };
            
            var resource3 = new ApiResource
            {
                AllowedAccessTokenSigningAlgorithms = resource3Algorithms
            };

            if (expectedAlgorithms.Any())
            {
                var allowedAlgorithms = new List<ApiResource> {resource1, resource2, resource3}.FindMatchingSigningAlgorithms();
                allowedAlgorithms.Should().BeEquivalentTo(expectedAlgorithms);
            }
            else
            {
                Action act = () => new List<ApiResource> {resource1, resource2, resource3}.FindMatchingSigningAlgorithms();
                act.Should().Throw<InvalidOperationException>();
            }
        }

        [Fact]
        public void Null_resource_list_should_throw_ArgumentNullException()
        {
            List<ApiResource> resources = null;
            Action act = () => resources.FindMatchingSigningAlgorithms();
            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Empty_resource_list_should_return_empty_collection()
        {
            var resources = new List<ApiResource>();
            var result = resources.FindMatchingSigningAlgorithms();
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [Fact]
        public void Resource_with_null_algorithms_should_be_skipped()
        {
            var resource1 = new ApiResource
            {
                AllowedAccessTokenSigningAlgorithms = null
            };
            var resource2 = new ApiResource
            {
                AllowedAccessTokenSigningAlgorithms = new[] { "RS256" }
            };

            var result = new List<ApiResource> { resource1, resource2 }.FindMatchingSigningAlgorithms();
            result.Should().BeEquivalentTo(new[] { "RS256" });
        }

        [Fact]
        public void Resource_with_empty_algorithms_should_be_skipped()
        {
            var resource1 = new ApiResource
            {
                AllowedAccessTokenSigningAlgorithms = new string[] { }
            };
            var resource2 = new ApiResource
            {
                AllowedAccessTokenSigningAlgorithms = new[] { "RS256" }
            };

            var result = new List<ApiResource> { resource1, resource2 }.FindMatchingSigningAlgorithms();
            result.Should().BeEquivalentTo(new[] { "RS256" });
        }
    }
}
