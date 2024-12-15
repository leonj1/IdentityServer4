// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4;
using IdentityServer4.Hosting.LocalApiAuthentication;
using Microsoft.AspNetCore.Authentication;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Extensions for registering the local access token authentication handler
    /// </summary>
    public static class LocalApiAuthenticationExtensions
    {
        /// <summary>
        /// Registers the authentication handler for local APIs.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <returns></returns>
        public static AuthenticationBuilder AddLocalApi(this AuthenticationBuilder builder)
            => builder.AddLocalApi(IdentityServerConstants.LocalApi.AuthenticationScheme, _ => { });

        /// <summary>
        /// Registers the authentication handler for local APIs.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="configureOptions">The configure options.</param>
        /// <returns></returns>
        public static AuthenticationBuilder AddLocalApi(this AuthenticationBuilder builder, Action<LocalApiAuthenticationOptions> configureOptions)
            => builder.AddLocalApi(IdentityServerConstants.LocalApi.AuthenticationScheme, configureOptions);

        /// <summary>
        /// Registers the authentication handler for local APIs.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="authenticationScheme">The authentication scheme.</param>
        /// <param name="configureOptions">The configure options.</param>
        /// <returns></returns>
        public static AuthenticationBuilder AddLocalApi(this AuthenticationBuilder builder, string authenticationScheme, Action<LocalApiAuthenticationOptions> configureOptions)
            => builder.AddLocalApi(authenticationScheme, displayName: null, configureOptions: configureOptions);

        /// <summary>
        /// Registers the authentication handler for local APIs.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="authenticationScheme">The authentication scheme.</param>
        /// <param name="displayName">The display name of this scheme.</param>
        /// <param name="configureOptions">The configure options.</param>
        /// <returns></returns>
        public static AuthenticationBuilder AddLocalApi(this AuthenticationBuilder builder, string authenticationScheme, string displayName, Action<LocalApiAuthenticationOptions> configureOptions)
        {
            return builder.AddScheme<LocalApiAuthenticationOptions, LocalApiAuthenticationHandler>(authenticationScheme, displayName, configureOptions);
        }
    }
}
