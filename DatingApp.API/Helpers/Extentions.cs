using System;

namespace DatingApp.API.Helpers
{
    public static class Extentions
    {
        public static int CalcAge(this DateTime from)
        {
            var age = DateTime.UtcNow.Year - from.Year;

            return from.AddYears(age) > DateTime.Today ? --age : age;
        }
    }
}