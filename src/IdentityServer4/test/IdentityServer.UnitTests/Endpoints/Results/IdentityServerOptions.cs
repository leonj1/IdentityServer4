// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace IdentityServer4.Configuration
{
    public class IdentityServerOptions
    {
        public AuthenticationOptions Authentication { get; set; } = new AuthenticationOptions();
        public CspOptions Csp { get; set; } = new CspOptions();
    }

    public class AuthenticationOptions
    {
        public string CheckSessionCookieName { get; set; }
    }

    public class CspOptions
    {
        public CspLevel Level { get; set; } = CspLevel.One;
        public bool AddDeprecatedHeader { get; set; } = true;
    }

    public enum CspLevel
    {
        One,
        Two
    }
}
