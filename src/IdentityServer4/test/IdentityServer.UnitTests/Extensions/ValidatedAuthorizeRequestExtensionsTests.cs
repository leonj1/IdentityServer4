using IdentityModel;
using IdentityServer4.Validation;
using System.Collections.Specialized;
using System.Linq;
using Xunit;

namespace IdentityServer.UnitTests.Extensions
{
    public class ValidatedAuthorizeRequestExtensionsTests
    {
        [Fact]
        public void GetAcrValues_should_return_snapshot_of_values()
        {
            var request = new ValidatedAuthorizeRequest()
            {
                Raw = new NameValueCollection()
            };
            request.AuthenticationContextReferenceClasses.Add("a");
            request.AuthenticationContextReferenceClasses.Add("b");
            request.AuthenticationContextReferenceClasses.Add("c");

            var acrs = request.GetAcrValues();
            foreach(var acr in acrs)
            {
                request.RemoveAcrValue(acr);
            }
        }

        [Fact]
        public void RemovePrompt_should_clear_prompt_values()
        {
            var request = new ValidatedAuthorizeRequest
            {
                Raw = new NameValueCollection
                {
                    { OidcConstants.AuthorizeRequest.Prompt, "login consent" }
                },
                PromptModes = new[] { "login", "consent" }
            };

            request.RemovePrompt();

            Assert.Empty(request.PromptModes);
            Assert.False(request.Raw.AllKeys.Contains(OidcConstants.AuthorizeRequest.Prompt));
        }

        [Fact]
        public void GetPrefixedAcrValue_should_return_value_when_prefix_exists()
        {
            var request = new ValidatedAuthorizeRequest
            {
                Raw = new NameValueCollection(),
                AuthenticationContextReferenceClasses = new List<string> { "idp:google", "tenant:123" }
            };

            var idp = request.GetPrefixedAcrValue("idp:");
            Assert.Equal("google", idp);
        }

        [Fact]
        public void GetPrefixedAcrValue_should_return_null_when_prefix_not_found()
        {
            var request = new ValidatedAuthorizeRequest
            {
                Raw = new NameValueCollection(),
                AuthenticationContextReferenceClasses = new List<string> { "idp:google" }
            };

            var tenant = request.GetPrefixedAcrValue("tenant:");
            Assert.Null(tenant);
        }

        [Fact]
        public void RemovePrefixedAcrValue_should_remove_matching_values()
        {
            var request = new ValidatedAuthorizeRequest
            {
                Raw = new NameValueCollection(),
                AuthenticationContextReferenceClasses = new List<string> { "idp:google", "tenant:123", "acr1" }
            };

            request.RemovePrefixedAcrValue("idp:");

            Assert.DoesNotContain(request.AuthenticationContextReferenceClasses, x => x.StartsWith("idp:"));
            Assert.Contains("tenant:123", request.AuthenticationContextReferenceClasses);
            Assert.Contains("acr1", request.AuthenticationContextReferenceClasses);
        }

        [Fact]
        public void GenerateSessionStateValue_should_return_null_for_non_openid_request()
        {
            var request = new ValidatedAuthorizeRequest
            {
                IsOpenIdRequest = false,
                ClientId = "client",
                SessionId = "session",
                RedirectUri = "https://client.com/callback"
            };

            var result = request.GenerateSessionStateValue();
            Assert.Null(result);
        }

        [Fact]
        public void GenerateSessionStateValue_should_return_value_for_valid_request()
        {
            var request = new ValidatedAuthorizeRequest
            {
                IsOpenIdRequest = true,
                ClientId = "client",
                SessionId = "session",
                RedirectUri = "https://client.com/callback"
            };

            var result = request.GenerateSessionStateValue();
            Assert.NotNull(result);
            Assert.Contains(".", result);
        }

        [Fact]
        public void GetAcrValues_should_return_empty_collection_when_no_values()
        {
            var request = new ValidatedAuthorizeRequest()
            {
                Raw = new NameValueCollection(),
                AuthenticationContextReferenceClasses = new List<string>()
            };

            var acrs = request.GetAcrValues();
            Assert.Empty(acrs);
        }

        [Fact]
        public void RemoveAcrValue_should_remove_specific_value()
        {
            var request = new ValidatedAuthorizeRequest()
            {
                Raw = new NameValueCollection(),
                AuthenticationContextReferenceClasses = new List<string> { "value1", "value2", "value3" }
            };

            request.RemoveAcrValue("value2");
            
            Assert.Equal(2, request.AuthenticationContextReferenceClasses.Count);
            Assert.Contains("value1", request.AuthenticationContextReferenceClasses);
            Assert.Contains("value3", request.AuthenticationContextReferenceClasses);
            Assert.DoesNotContain("value2", request.AuthenticationContextReferenceClasses);
        }

        [Fact]
        public void GenerateSessionStateValue_should_return_null_when_missing_required_fields()
        {
            var request = new ValidatedAuthorizeRequest
            {
                IsOpenIdRequest = true,
                ClientId = "client",
                // Missing SessionId
                RedirectUri = "https://client.com/callback"
            };

            var result = request.GenerateSessionStateValue();
            Assert.Null(result);
        }

        [Fact]
        public void GetPrefixedAcrValue_should_handle_multiple_matching_prefixes()
        {
            var request = new ValidatedAuthorizeRequest
            {
                Raw = new NameValueCollection(),
                AuthenticationContextReferenceClasses = new List<string> 
                { 
                    "idp:google", 
                    "idp:facebook",
                    "tenant:123" 
                }
            };

            var idp = request.GetPrefixedAcrValue("idp:");
            Assert.Equal("google", idp); // Should return first match
        }
    }
}
