// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4.Extensions;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace IdentityServer4.Configuration
{
    internal class PostConfigureInternalCookieOptions : IPostConfigureOptions<CookieAuthenticationOptions>
    {
        private readonly IdentityServerOptions _idsrv;
        private readonly IOptions<Microsoft.AspNetCore.Authentication.AuthenticationOptions> _authOptions;
        private readonly ILogger _logger;

        public PostConfigureInternalCookieOptions(
            IdentityServerOptions idsrv,
            IOptions<Microsoft.AspNetCore.Authentication.AuthenticationOptions> authOptions,
            ILoggerFactory loggerFactory)
        {
            _idsrv = idsrv;
            _authOptions = authOptions;
            _logger = loggerFactory.CreateLogger("IdentityServer4.Startup");
        }

        public void PostConfigure(string name, CookieAuthenticationOptions options)
        {
            var scheme = _idsrv.Authentication.CookieAuthenticationScheme ??
                _authOptions.Value.DefaultAuthenticateScheme ??
                _authOptions.Value.DefaultScheme;

            if (name == scheme)
            {
                _idsrv.UserInteraction.LoginUrl = _idsrv.UserInteraction.LoginUrl ?? options.LoginPath;
                _idsrv.UserInteraction.LoginReturnUrlParameter = _idsrv.UserInteraction.LoginReturnUrlParameter ?? options.ReturnUrlParameter;
                _idsrv.UserInteraction.LogoutUrl = _idsrv.UserInteraction.LogoutUrl ?? options.LogoutPath;

                _logger.LogDebug("Login Url: {url}", _idsrv.UserInteraction.LoginUrl);
                _logger.LogDebug("Login Return Url Parameter: {param}", _idsrv.UserInteraction.LoginReturnUrlParameter);
                _logger.LogDebug("Logout Url: {url}", _idsrv.UserInteraction.LogoutUrl);

                _logger.LogDebug("ConsentUrl Url: {url}", _idsrv.UserInteraction.ConsentUrl);
                _logger.LogDebug("Consent Return Url Parameter: {param}", _idsrv.UserInteraction.ConsentReturnUrlParameter);

                _logger.LogDebug("Error Url: {url}", _idsrv.UserInteraction.ErrorUrl);
                _logger.LogDebug("Error Id Parameter: {param}", _idsrv.UserInteraction.ErrorIdParameter);
            }
        }
    }
}
