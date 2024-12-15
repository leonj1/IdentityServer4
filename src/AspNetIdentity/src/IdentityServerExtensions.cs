using System;
using System.Linq;
using IdentityModel;
using IdentityServer4;
using IdentityServer4.AspNetIdentity;
using IdentityServer4.Configuration;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Extension methods to add ASP.NET Identity support to IdentityServer.
    /// </summary>
    public static class IdentityServerBuilderExtensions
    {
        /// <summary>
        /// Configures IdentityServer to use the ASP.NET Identity implementations 
        /// of IUserClaimsPrincipalFactory, IResourceOwnerPasswordValidator, and IProfileService.
        /// Also configures some of ASP.NET Identity's options for use with IdentityServer (such as claim types to use
        /// and authentication cookie settings).
        /// </summary>
        /// <typeparam name="TUser">The type of the user.</typeparam>
        /// <param name="builder">The builder.</param>
        /// <returns></returns>
        public static IIdentityServerBuilder AddAspNetIdentity<TUser>(this IIdentityServerBuilder builder)
            where TUser : class
        {
            builder.Services.AddTransientDecorator<IUserClaimsPrincipalFactory<TUser>, UserClaimsFactory<TUser>>();

            builder.Services.Configure<IdentityOptions>(options =>
            {
                options.ClaimsIdentity.UserIdClaimType = JwtClaimTypes.Subject;
                options.ClaimsIdentity.UserNameClaimType = JwtClaimTypes.Name;
                options.ClaimsIdentity.RoleClaimType = JwtClaimTypes.Role;
            });

            builder.Services.Configure<SecurityStampValidatorOptions>(opts =>
            {
                opts.OnRefreshingPrincipal = SecurityStampValidatorCallback.UpdatePrincipal;
            });

            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.AuthenticationScheme = IdentityServerConstants.DefaultAuthenticationScheme;
                options.LoginPath = "/Account/Login";
                options.AccessDeniedPath = "/Account/AccessDenied";
                options.SlidingExpiration = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
                options.Cookie.Name = IdentityServerConstants.DefaultCookieName;
            });

            builder.AddResourceOwnerValidator<ResourceOwnerPasswordValidator<TUser>>();
            builder.AddProfileService<ProfileService<TUser>>();

            return builder;
        }

        internal static void AddTransientDecorator<TService, TImplementation>(this IServiceCollection services)
            where TService : class
            where TImplementation : class, TService
        {
            services.AddDecorator<TService>();
            services.AddTransient<TService, TImplementation>();
        }

        internal static void AddDecorator<TService>(this IServiceCollection services)
        {
            var registration = services.LastOrDefault(x => x.ServiceType == typeof(TService));
            if (registration == null)
                throw new ArgumentException($"No service of type {typeof(TService)} registered.");

            services.Remove(registration);

            if (registration.ImplementationInstance != null)
            {
                var innerType = typeof(Decorator<,>).MakeGenericType(typeof(TService), registration.ImplementationInstance.GetType());
                services.Add(new ServiceDescriptor(typeof(Decorator<TService>), innerType, ServiceLifetime.Transient));
                services.Add(new ServiceDescriptor(registration.ImplementationInstance.GetType(), registration.ImplementationInstance));
            }
            else if (registration.ImplementationFactory != null)
            {
                services.Add(new ServiceDescriptor(typeof(Decorator<TService>), provider =>
                {
                    return new DisposableDecorator<TService>((TService)registration.ImplementationFactory(provider));
                }, registration.Lifetime));
            }
            else
            {
                var innerType = typeof(Decorator<,>).MakeGenericType(typeof(TService), registration.ImplementationType);
                services.Add(new ServiceDescriptor(typeof(Decorator<TService>), innerType, ServiceLifetime.Transient));
                services.Add(new ServiceDescriptor(registration.ImplementationType, registration.ImplementationType, registration.Lifetime));
            }
        }
    }

    internal class Decorator<TService, TImplementation> : IDisposable
        where TService : class
        where TImplementation : class, TService
    {
        private readonly TService _inner;
        private readonly Action<TService> _dispose;

        public Decorator(TService inner, Action<TService> dispose)
        {
            _inner = inner ?? throw new ArgumentNullException(nameof(inner));
            _dispose = dispose ?? throw new ArgumentNullException(nameof(dispose));
        }

        public TService Inner => _inner;

        public void Dispose()
        {
            _dispose(_inner);
        }
    }

    internal class DisposableDecorator<TService> : Decorator<TService, TService>
        where TService : class
    {
        public DisposableDecorator(TService inner) : base(inner, s => ((IDisposable)s).Dispose())
        {
        }
    }
}
