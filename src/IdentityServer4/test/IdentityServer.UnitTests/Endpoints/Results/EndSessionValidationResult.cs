using System;

namespace IdentityServer4.Validation
{
    public class EndSessionValidationResult
    {
        public bool IsError { get; set; }
        public ValidatedEndSessionRequest ValidatedRequest { get; set; }

        public EndSessionValidationResult()
        {
            IsError = false;
        }
    }
}
