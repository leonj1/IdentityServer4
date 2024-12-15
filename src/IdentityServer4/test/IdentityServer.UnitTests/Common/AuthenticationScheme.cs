using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;

namespace IdentityServer.UnitTests.Common
{
    public class AuthenticationScheme
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public Type HandlerType { get; set; }

        public AuthenticationScheme(string name, string displayName, Type handlerType)
        {
            Name = name;
            DisplayName = displayName;
            HandlerType = handlerType;
        }
    }
}
