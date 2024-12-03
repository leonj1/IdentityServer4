using System;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using IdentityServer4.Validation;
using Microsoft.Extensions.Logging;
using Xunit;

namespace IdentityServer.UnitTests.Validation
{
    public class TokenRequestValidation_DeviceCode_Invalid
    {
        private readonly IDeviceCodeValidator _validator;
        private readonly IDeviceFlowStore _deviceFlowStore;
        private readonly ILogger<DeviceCodeValidator> _logger;

        public TokenRequestValidation_DeviceCode_Invalid()
        {
            _deviceFlowStore = new InMemoryDeviceFlowStore();
            _logger = new TestLogger<DeviceCodeValidator>();
            _validator = new DeviceCodeValidator(_deviceFlowStore, _logger);
        }

        [Fact]
        public async Task Invalid_DeviceCode_Should_Fail()
        {
            var request = new ValidatedTokenRequest();
            request.DeviceCode = "invalid_device_code";

            var result = await _validator.ValidateAsync(request);

            result.IsError.Should().BeTrue();
            result.Error.Should().Be("invalid_grant");
        }

        [Fact]
        public async Task Expired_DeviceCode_Should_Fail()
        {
            var deviceCode = new DeviceCode
            {
                ClientId = "client1",
                CreationTime = DateTime.UtcNow.AddHours(-1),
                Lifetime = 300 // 5 minutes
            };

            await _deviceFlowStore.StoreDeviceAuthorizationAsync("device_code", deviceCode);

            var request = new ValidatedTokenRequest();
            request.DeviceCode = "device_code";

            var result = await _validator.ValidateAsync(request);

            result.IsError.Should().BeTrue();
            result.Error.Should().Be("expired_token");
        }

        [Fact]
        public async Task Pending_DeviceCode_Should_Fail()
        {
            var deviceCode = new DeviceCode
            {
                ClientId = "client1",
                CreationTime = DateTime.UtcNow,
                Lifetime = 300,
                IsAuthorized = false
            };

            await _deviceFlowStore.StoreDeviceAuthorizationAsync("device_code", deviceCode);

            var request = new ValidatedTokenRequest();
            request.DeviceCode = "device_code";

            var result = await _validator.ValidateAsync(request);

            result.IsError.Should().BeTrue();
            result.Error.Should().Be("authorization_pending");
        }
    }
}
