using System;

namespace Library.API.Extensions
{
    public static class DateTimeOffsetExtensions
    {
        public static int GetCurrentAge(
            this DateTimeOffset dateTimeOffset, 
            DateTimeOffset? dateOfDeath)
        {
            var dateToCalculate = dateOfDeath?.UtcDateTime ?? DateTime.UtcNow;

            int age = dateToCalculate.Year - dateTimeOffset.Year;

            if (dateToCalculate < dateTimeOffset.AddYears(age))
            {
                age--;
            }

            return age;
        }
    }
}
