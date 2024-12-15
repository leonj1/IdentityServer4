namespace IdentityServer4.Validation
{
    public class EndSessionCallbackValidationResult
    {
        public bool IsError { get; set; }
        public string[] FrontChannelLogoutUrls { get; set; }
    }
}
