using System;

namespace SubastaYa.Core.Utils
{
    public static class ArgTime
    {
        public static DateTime Now => DateTime.UtcNow.AddHours(-3);
    }
}