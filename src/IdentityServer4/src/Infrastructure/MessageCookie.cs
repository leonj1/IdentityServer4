// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4.Configuration;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace IdentityServer4
{
    internal class MessageCookie<TModel>
    {
        private readonly ILogger _logger;
        private readonly IdentityServerOptions _options;
        private readonly IHttpContextAccessor _context;
        private readonly IDataProtector _protector;

        public MessageCookie(
            ILogger<MessageCookie<TModel>> logger, 
            IdentityServerOptions options,
            IHttpContextAccessor context, 
            IDataProtectionProvider provider)
        {
            _logger = logger;
            _options = options;
            _context = context;
            _protector = provider.CreateProtector(MessageType);
        }

        private string MessageType => typeof(TModel).Name;

        private string Protect(Message<TModel> message)
        {
            var json = ObjectSerializer.ToString(message);
            _logger.LogTrace("Protecting message: {0}", json);

            return _protector.Protect(json);
        }

        private Message<TModel> Unprotect(string data)
        {
            try
            {
                var json = _protector.Unprotect(data);
                return ObjectSerializer.FromJson<Message<TModel>>(json);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error unprotecting message cookie");
                ClearByCookieName(name);
                return null;
            }
        }

        protected internal void Clear(string id)
        {
            var name = GetCookieFullName(id);
            ClearByCookieName(name);
        }

        private void ClearByCookieName(string name)
        {
            _context.HttpContext.Response.Cookies.Append(
                name,
                ".",
                new CookieOptions
                {
                    Expires = new DateTime(2000, 1, 1),
                    HttpOnly = true,
                    Secure = Secure,
                    Path = CookiePath,
                    IsEssential = true
                });
        }

        private long GetCookieRank(string name)
        {   
            // empty and invalid cookies are considered to be the oldest:
            var rank = DateTime.MinValue.Ticks;

            try
            {
                var message = ReadByCookieName(name);
                if (message != null)
                {
                    // valid cookies are ranked based on their creation time:
                    rank = message.Created;
                }
            }
            catch (CryptographicException e)
            {   
                // cookie was protected with a different key/algorithm
                _logger.LogDebug(e, "Unable to unprotect cookie {0}", name);
            }
            
            return rank;
        }

        private void ClearOverflow()
        {
            var names = GetCookieNames();
            var toKeep = _options.UserInteraction.CookieMessageThreshold;

            if (names.Count() >= toKeep)
            {
                var rankedCookieNames =
                    from name in names
                    let rank = GetCookieRank(name)
                    orderby rank descending
                    select name;

                var purge = rankedCookieNames.Skip(Math.Max(0, toKeep - 1));
                foreach (var name in purge)
                {
                    _logger.LogTrace("Purging stale cookie: {cookieName}", name);
                    ClearByCookieName(name);
                }
            }
        }

        private string GetCookieFullName(string id)
        {
            return $"{_options.UserInteraction.CookieMessagePrefix}_{id}";
        }

        private bool Secure => _context.HttpContext.Request.IsHttps;

        private string CookiePath => _options.UserInteraction.CookieMessagePath ?? "/";

        private IEnumerable<string> GetCookieNames()
        {
            return _context.HttpContext.Request.Cookies.Keys
                .Where(k => k.StartsWith(_options.UserInteraction.CookieMessagePrefix));
        }

        private Message<TModel> ReadByCookieName(string name)
        {
            var data = _context.HttpContext.Request.Cookies[name];
            if (data == null)
            {
                return null;
            }
            return Unprotect(data);
        }
    }
}
