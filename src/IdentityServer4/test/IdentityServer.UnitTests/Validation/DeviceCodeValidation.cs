using System;

namespace IdentityServerExample
{
    public class IdentityServerUser
    {
        public string Username { get; set; }
        public string Password { get; set; }

        public bool Validate(string username, string password)
        {
            return Username == username && Password == password;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            IdentityServerUser user = new IdentityServerUser
            {
                Username = "admin",
                Password = "password"
            };

            bool isValid = user.Validate("admin", "password");
            Console.WriteLine(isValid);
        }
    }
}
