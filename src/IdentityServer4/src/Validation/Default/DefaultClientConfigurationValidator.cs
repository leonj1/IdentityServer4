public class DefaultClientConfigurationValidator
{
    public static bool Validate(ClientConfiguration config)
    {
        if (config == null)
        {
            return false;
        }

        if (string.IsNullOrEmpty(config.ApiKey))
        {
            return false;
        }

        if (config.Timeout <= 0)
        {
            return false;
        }

        return true;
    }
}

public class ClientConfiguration
{
    public string ApiKey { get; set; }
    public int Timeout { get; set; }
}
