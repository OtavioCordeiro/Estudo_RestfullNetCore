using System;
using System.Collections.Generic;
using System.Linq;

namespace Library.API.Extensions
{
    public static class GuidExtensions
    {
        public static bool IsEmpty(this IEnumerable<Guid> guids)
        {
            if (guids.Any() == false || guids.All(g => g == Guid.Empty))
                return true;

            return false;
        }
    }
}
