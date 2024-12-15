using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace IdentityServer.IntegrationTests.Common
{
    public class CustomCookieContainer : CookieContainer
    {
        // Existing code from BrowserHandler.cs
        public override void Add(Uri uri, Cookie cookie)
        {
            base.Add(uri, cookie);
        }

        public override void SetCookies(Uri uri, string cookies)
        {
            base.SetCookies(uri, cookies);
        }

        public override CookieCollection GetCookies(Uri uri)
        {
            return base.GetCookies(uri);
        }
    }
}
