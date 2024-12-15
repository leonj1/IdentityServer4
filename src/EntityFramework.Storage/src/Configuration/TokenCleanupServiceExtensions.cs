using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Interfaces;
using System;
using IdentityServer4.EntityFramework.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityServer4.EntityFramework.Storage
{
    public static class TokenCleanupServiceExtensions
    {
        /// <summary>
        /// Adds operational DbContext to the DI system.
        /// </summary>
        /// <typeparam name="TContext"></typeparam>
        /// <param name="services"></param>
        /// <param name="storeOptionsAction">The store options action.</param>
        /// <returns></returns>
        public static IServiceCollection AddOperationalDbContext<TContext>(this IServiceCollection services,
            Action<OperationalStoreOptions> storeOptionsAction = null)
            where TContext : DbContext, IPersistedGrantDbContext
        {
            var storeOptions = new OperationalStoreOptions();
            services.AddSingleton(storeOptions);
            storeOptionsAction?.Invoke(storeOptions);

            if (storeOptions.ResolveDbContextOptions != null)
            {
                services.AddDbContext<TContext>(storeOptions.ResolveDbContextOptions);
            }
            else
            {
                services.AddDbContext<TContext>(dbCtxBuilder =>
                {
                    storeOptions.ConfigureDbContext?.Invoke(dbCtxBuilder);
                });
            }

            services.AddScoped<IPersistedGrantDbContext, TContext>();
            services.AddTransient<TokenCleanupService>();

            return services;
        }
    }
}
