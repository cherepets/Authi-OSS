using System.Collections.Generic;
using System.Linq;

namespace Authi.Common.Extensions
{
    public static class EnumerableExt
    {
        public static IReadOnlyCollection<T> ToReadOnly<T>(this IEnumerable<T> enumerable) 
            => enumerable.ToList();
    }
}
