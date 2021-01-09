using System;
using System.Collections.Generic;
using System.Linq;

namespace fairTeams.Core
{
    public static class EnumerableExtensions
    {
        public static IList<T> Randomize<T>(this IEnumerable<T> source)
        {
            var random = new Random();
            return source.OrderBy((item) => random.Next()).ToList();
        }
    }
}
