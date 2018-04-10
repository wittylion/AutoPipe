using System.Collections.Generic;
using System.Linq;

namespace Pipelines.ExtensionMethods
{
    internal static class EnumerableExtensionMethods
    {
        public static IEnumerable<T> EnsureAtLeastEmpty<T>(this IEnumerable<T> enumerable)
        {
            return enumerable.Ensure(Enumerable.Empty<T>());
        }
    }
}