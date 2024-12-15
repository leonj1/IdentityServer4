// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4.Extensions;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace IdentityServer4.Configuration
{
    internal class ConfigureInternalCookieOptions : IConfigureNamedOptions<CookieAuthenticationOptions>
    {
        private readonly IdentityServerOptions _idsrv;

        public ConfigureInternalCookieOptions(IdentityServerOptions idsrv)
        {
            _idsrv = idsrv;
        }

        public void Configure(CookieAuthenticationOptions options)
        {
        }

        public void Configure(string name, CookieAuthenticationOptions options)
        {
            if (name == IdentityServerConstants.DefaultCookieAuthenticationScheme)
            {
                options.SlidingExpiration = _idsrv.Authentication.CookieSlidingExpiration;
                options.ExpireTimeSpan = _idsrv.Authentication.CookieLifetime;
                options.Cookie.Name = IdentityServerConstants.DefaultCookieAuthenticationScheme;
                options.Cookie.IsEssential = true;
                options.Cookie.SameSite = _idsrv.Authentication.CookieSameSiteMode;

                options.LoginPath = ExtractLocalUrl(_idsrv.UserInteraction.LoginUrl);
                options.LogoutPath = ExtractLocalUrl(_idsrv.UserInteraction.LogoutUrl);
                if (_idsrv.UserInteraction.LoginReturnUrlParameter != null)
                {
                    options.ReturnUrlParameter = _idsrv.UserInteraction.LoginReturnUrlParameter;
                }
            }

            if (name == IdentityServerConstants.ExternalCookieAuthenticationScheme)
            {
                options.Cookie.Name = IdentityServerConstants.ExternalCookieAuthenticationScheme;
                options.Cookie.IsEssential = true;
                options.Cookie.SameSite = _idsrv.Authentication.CookieSameSiteMode;

                options.LoginPath = ExtractLocalUrl(_idsrv.UserInteraction.LoginUrl);
                options.LogoutPath = ExtractLocalUrl(_idsrv.UserInteraction.LogoutUrl);
            }
        }

        private string ExtractLocalUrl(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return null;
            }

            var uri = new Uri(url);
            if (uri.IsAbsoluteUri && uri.Scheme == "http" || uri.Scheme == "https")
            {
                return uri.PathAndQuery;
            }

            return url;
        }
    }
}
