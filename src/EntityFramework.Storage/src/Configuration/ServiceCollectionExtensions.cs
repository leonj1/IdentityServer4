using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Interfaces;
using System;
using IdentityServer4.EntityFramework.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityServer4.EntityFramework.Storage
{
    /// <summary>
    /// Extension methods to add EF database support to IdentityServer.
    /// </summary>
    public static class IdentityServerEntityFrameworkBuilderExtensions
    {
        /// <summary>
        /// Add Configuration DbContext to the DI system.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="storeOptionsAction">The store options action.</param>
        /// <returns></returns>
        public static IServiceCollection AddConfigurationDbContext(this IServiceCollection services,
            Action<ConfigurationStoreOptions> storeOptionsAction = null)
        {
            return services.AddConfigurationDbContext<ConfigurationDbContext>(storeOptionsAction);
        }

        /// <summary>
        /// Adds operational DbContext to the DI system.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="storeOptionsAction">The store options action.</param>
        /// <returns></returns>
        public static IServiceCollection AddOperationalDbContext(this IServiceCollection services,
            Action<OperationalStoreOptions> storeOptionsAction = null)
        {
            return services.AddOperationalDbContext<PersistedGrantDbContext>(storeOptionsAction);
        }

        /// <summary>
        /// Adds an implementation of the IOperationalStoreNotification to the DI system.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddOperationalStoreNotification<T>(this IServiceCollection services)
           where T : class, IOperationalStoreNotification
        {
            services.AddTransient<IOperationalStoreNotification, T>();
            return services;
        }
    }
}
