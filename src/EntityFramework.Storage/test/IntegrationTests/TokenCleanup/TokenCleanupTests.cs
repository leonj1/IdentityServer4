using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.EntityFramework.Interfaces;
using IdentityServer4.EntityFramework.Options;
using IdentityServer4.EntityFramework.Stores;
using IdentityServer4.Stores;
using IdentityServer4.Test;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace IdentityServer4.EntityFramework.IntegrationTests.TokenCleanup
{
    public class TokenCleanupTests
    {
        [Theory, MemberData(nameof(TestDatabaseProviders))]
        public async Task RemoveExpiredGrantsAsync_WhenExpiredDeviceGrantsExist_ExpectExpiredDeviceGrantsRemoved(DbContextOptions<PersistedGrantDbContext> options)
        {
            var expiredGrant = new DeviceFlowCodes
            {
                DeviceCode = Guid.NewGuid().ToString(),
                UserCode = Guid.NewGuid().ToString(),
                ClientId = "app1",
                SubjectId = "123",
                CreationTime = DateTime.UtcNow.AddDays(-4),
                Expiration = DateTime.UtcNow.AddDays(-3),
                Data = "{!}"
            };

            using (var context = new PersistedGrantDbContext(options, StoreOptions))
            {
                context.DeviceFlowCodes.Add(expiredGrant);
                await context.SaveChangesAsync();
            }

            var sut = CreateSut(options);
            await sut.RemoveExpiredGrantsAsync();

            using (var context = new PersistedGrantDbContext(options, StoreOptions))
            {
                var result = await context.DeviceFlowCodes.FirstOrDefaultAsync(x => x.DeviceCode == expiredGrant.DeviceCode);
                result.Should().BeNull();
            }
        }

        [Theory, MemberData(nameof(TestDatabaseProviders))]
        public async Task RemoveExpiredGrantsAsync_WhenValidDeviceGrantsExist_ExpectValidDeviceGrantsInDb(DbContextOptions<PersistedGrantDbContext> options)
        {
            var validGrant = new DeviceFlowCodes
            {
                DeviceCode = Guid.NewGuid().ToString(),
                UserCode = "2468",
                ClientId = "app1",
                SubjectId = "123",
                CreationTime = DateTime.UtcNow.AddDays(-4),
                Expiration = DateTime.UtcNow.AddDays(3),
                Data = "{!}"
            };

            using (var context = new PersistedGrantDbContext(options, StoreOptions))
            {
                context.DeviceFlowCodes.Add(validGrant);
                await context.SaveChangesAsync();
            }

            var sut = CreateSut(options);
            await sut.RemoveExpiredGrantsAsync();

            using (var context = new PersistedGrantDbContext(options, StoreOptions))
            {
                var result = await context.DeviceFlowCodes.FirstOrDefaultAsync(x => x.DeviceCode == validGrant.DeviceCode);
                result.Should().NotBeNull();
            }
        }

        [Theory, MemberData(nameof(TestDatabaseProviders))]
        public async Task RemoveExpiredGrantsAsync_WhenExpiredGrantsExist_ExpectExpiredGrantsRemoved(DbContextOptions<PersistedGrantDbContext> options)
        {
            var expiredGrant = new PersistedGrant
            {
                Key = Guid.NewGuid().ToString(),
                Type = "refresh_token",
                ClientId = "app1",
                SubjectId = "123",
                CreationTime = DateTime.UtcNow.AddDays(-4),
                Expiration = DateTime.UtcNow.AddDays(-3),
                Data = "{!}"
            };

            using (var context = new PersistedGrantDbContext(options, StoreOptions))
            {
                context.PersistedGrants.Add(expiredGrant);
                await context.SaveChangesAsync();
            }

            var sut = CreateSut(options);
            await sut.RemoveExpiredGrantsAsync();

            using (var context = new PersistedGrantDbContext(options, StoreOptions))
            {
                var result = await context.PersistedGrants.FirstOrDefaultAsync(x => x.Key == expiredGrant.Key);
                result.Should().BeNull();
            }
        }

        [Theory, MemberData(nameof(TestDatabaseProviders))]
        public async Task RemoveExpiredGrantsAsync_WhenValidGrantsExist_ExpectValidGrantsInDb(DbContextOptions<PersistedGrantDbContext> options)
        {
            var validGrant = new PersistedGrant
            {
                Key = Guid.NewGuid().ToString(),
                Type = "refresh_token",
                ClientId = "app1",
                SubjectId = "123",
                CreationTime = DateTime.UtcNow.AddDays(-4),
                Expiration = DateTime.UtcNow.AddDays(3),
                Data = "{!}"
            };

            using (var context = new PersistedGrantDbContext(options, StoreOptions))
            {
                context.PersistedGrants.Add(validGrant);
                await context.SaveChangesAsync();
            }

            var sut = CreateSut(options);
            await sut.RemoveExpiredGrantsAsync();

            using (var context = new PersistedGrantDbContext(options, StoreOptions))
            {
                var result = await context.PersistedGrants.FirstOrDefaultAsync(x => x.Key == validGrant.Key);
                result.Should().NotBeNull();
            }
        }

        private EntityFramework.TokenCleanupService CreateSut(DbContextOptions<PersistedGrantDbContext> options)
        {
            IServiceCollection services = new ServiceCollection();
            services.AddIdentityServer()
                .AddTestUsers(new List<TestUser>())
                .AddInMemoryClients(new List<Models.Client>())
                .AddInMemoryIdentityResources(new List<Models.IdentityResource>())
                .AddInMemoryApiResources(new List<Models.ApiResource>());

            services.AddScoped<IPersistedGrantDbContext, PersistedGrantDbContext>(_ =>
                new PersistedGrantDbContext(options, StoreOptions));
            services.AddScoped<EntityFramework.TokenCleanupService>();

            var serviceProvider = services.BuildServiceProvider();
            return serviceProvider.GetService<EntityFramework.TokenCleanupService>();
        }

        public static IEnumerable<object[]> TestDatabaseProviders()
        {
            yield return new object[] { /* provide DbContextOptions here */ };
        }
    }
}
