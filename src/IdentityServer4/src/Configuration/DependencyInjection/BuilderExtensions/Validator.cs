namespace YourNamespace // Replace with your actual namespace
{
    public static class Validator
    {
        public static bool Validate(string input)
        {
            return !string.IsNullOrEmpty(input) && input.Length > 5;
        }
    }
}
