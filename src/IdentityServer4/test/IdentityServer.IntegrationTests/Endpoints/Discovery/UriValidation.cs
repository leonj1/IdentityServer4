namespace UriValidationNamespace
{
    public class UriValidation
    {
        public static bool Issuer_uri_should_be_lowercase(string uri)
        {
            return uri.ToLower() == uri;
        }
    }
}
