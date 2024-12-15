using System;

namespace TokenValidationTests
{
    public class Clock : IClock
    {
        public DateTime.UtcNow => DateTime.UtcNow;
    }
}
