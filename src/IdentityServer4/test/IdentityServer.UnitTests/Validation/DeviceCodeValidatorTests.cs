using System;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Xunit;
using System.Security.Claims;

namespace IdentityServer.UnitTests.Validation
{
    public class DeviceCodeValidatorTests
    {
        private readonly ISystemClock _clock;
        private readonly StubDeviceFlowCodeService _devices;
        private readonly StubDeviceFlowThrottlingService _throttlingService;
        private readonly StubProfileService _profile;
        private readonly DeviceCodeValidator _validator;

        public DeviceCodeValidatorTests()
        {
            _clock = new SystemClock();
            _devices = new StubDeviceFlowCodeService();
            _throttlingService = new StubDeviceFlowThrottlingService();
            _profile = new StubProfileService();
            _validator = new DeviceCodeValidator(
                _devices,
                _profile,
                _throttlingService,
                _clock,
                new LoggerFactory().CreateLogger<DeviceCodeValidator>());
        }

        [Fact]
        public async Task Invalid_Device_Code_Should_Fail()
        {
            var context = new DeviceCodeValidationContext 
            { 
                DeviceCode = "invalid_code",
                Request = new ValidatedTokenRequest()
            };

            await _validator.ValidateAsync(context);

            context.Result.IsError.Should().BeTrue();
            context.Result.Error.Should().Be(OidcConstants.TokenErrors.InvalidGrant);
        }

        [Fact]
        public async Task Valid_Code_But_Wrong_Client_Should_Fail()
        {
            var deviceCode = new DeviceCode
            {
                ClientId = "client1",
                CreationTime = DateTime.UtcNow,
                Lifetime = 300
            };

            _devices.DeviceCode = deviceCode;

            var context = new DeviceCodeValidationContext
            {
                DeviceCode = "valid_code",
                Request = new ValidatedTokenRequest
                {
                    Client = new Client { ClientId = "client2" }
                }
            };

            await _validator.ValidateAsync(context);

            context.Result.IsError.Should().BeTrue();
            context.Result.Error.Should().Be(OidcConstants.TokenErrors.InvalidGrant);
        }

        [Fact]
        public async Task Expired_Code_Should_Fail()
        {
            var deviceCode = new DeviceCode
            {
                ClientId = "client1",
                CreationTime = DateTime.UtcNow.AddHours(-1),
                Lifetime = 300 // 5 minutes
            };

            _devices.DeviceCode = deviceCode;

            var context = new DeviceCodeValidationContext
            {
                DeviceCode = "valid_code",
                Request = new ValidatedTokenRequest
                {
                    Client = new Client { ClientId = "client1" }
                }
            };

            await _validator.ValidateAsync(context);

            context.Result.IsError.Should().BeTrue();
            context.Result.Error.Should().Be(OidcConstants.TokenErrors.ExpiredToken);
        }

        [Fact]
        public async Task Throttled_Request_Should_Fail()
        {
            var deviceCode = new DeviceCode
            {
                ClientId = "client1",
                CreationTime = DateTime.UtcNow,
                Lifetime = 300
            };

            _devices.DeviceCode = deviceCode;
            _throttlingService.ShouldSlowDown = true;

            var context = new DeviceCodeValidationContext
            {
                DeviceCode = "valid_code",
                Request = new ValidatedTokenRequest
                {
                    Client = new Client { ClientId = "client1" }
                }
            };

            await _validator.ValidateAsync(context);

            context.Result.IsError.Should().BeTrue();
            context.Result.Error.Should().Be(OidcConstants.TokenErrors.SlowDown);
        }
    }

    internal class StubDeviceFlowCodeService : IDeviceFlowCodeService
    {
        public DeviceCode DeviceCode { get; set; }

        public Task<DeviceCode> FindByDeviceCodeAsync(string deviceCode)
        {
            return Task.FromResult(DeviceCode);
        }

        public Task<DeviceCode> FindByUserCodeAsync(string userCode)
        {
            throw new NotImplementedException();
        }

        public Task RemoveByDeviceCodeAsync(string deviceCode)
        {
            return Task.CompletedTask;
        }

        public Task StoreDeviceAuthorizationAsync(string deviceCode, string userCode, DeviceCode data)
        {
            throw new NotImplementedException();
        }

        public Task UpdateByUserCodeAsync(string userCode, DeviceCode data)
        {
            throw new NotImplementedException();
        }
    }

    internal class StubDeviceFlowThrottlingService : IDeviceFlowThrottlingService
    {
        public bool ShouldSlowDown { get; set; }

        public Task<bool> ShouldSlowDown(string deviceCode, DeviceCode details)
        {
            return Task.FromResult(ShouldSlowDown);
        }
    }

    internal class StubProfileService : IProfileService
    {
        public bool IsActive { get; set; } = true;

        public Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            throw new NotImplementedException();
        }

        public Task IsActiveAsync(IsActiveContext context)
        {
            context.IsActive = IsActive;
            return Task.CompletedTask;
        }
    }
}
