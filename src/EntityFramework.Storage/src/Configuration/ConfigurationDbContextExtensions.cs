using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Interfaces;
using System;
using IdentityServer4.EntityFramework.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityServer4.EntityFramework.Storage
{
    public static class ConfigurationDbContextExtensions
    {
        /// <summary>
        /// Add Configuration DbContext to the DI system.
        /// </summary>
        /// <typeparam name="TContext"></typeparam>
        /// <param name="services"></param>
        /// <param name="storeOptionsAction">The store options action.</param>
        /// <returns></returns>
        public static IServiceCollection AddConfigurationDbContext<TContext>(this IServiceCollection services,
            Action<ConfigurationStoreOptions> storeOptionsAction = null)
            where TContext : DbContext, IConfigurationDbContext
        {
            var storeOptions = new ConfigurationStoreOptions();
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

            services.AddScoped<IConfigurationDbContext, TContext>();

            return services;
        }
    }
}
