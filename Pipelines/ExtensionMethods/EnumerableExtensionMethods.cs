using System.Collections.Generic;
using System.Linq;

namespace Pipelines.ExtensionMethods
{
    /// <summary>
    /// Extension methods for classes <see cref="IEnumerable{T}"/>.
    /// </summary>
    internal static class EnumerableExtensionMethods
    {
        /// <summary>
        /// If the <paramref name="enumerable"/> is <c>null</c> returns
        /// <see cref="Enumerable.Empty{TResult}"/> otherwise returns parameter itself.
        /// </summary>
        /// <typeparam name="T">
        /// Generic type of the parameter.
        /// </typeparam>
        /// <param name="enumerable">
        /// Collection to be checked for null.
        /// </param>
        /// <returns>
        /// Empty enumerable if the parameter is null, or enumerable object itself.
        /// </returns>
        public static IEnumerable<T> EnsureAtLeastEmpty<T>(this IEnumerable<T> enumerable)
        {
            return enumerable.Ensure(Enumerable.Empty<T>());
        }
        /// <summary>
        /// Opposite action of the
        /// <see cref="Enumerable.Any{TSource}(System.Collections.Generic.IEnumerable{TSource})"/>
        /// which checks presence of the items in the enumerable object.
        /// </summary>
        /// <remarks>
        /// Returns <c>false</c> if <paramref name="enumerable"/> is null.
        /// </remarks>
        /// <typeparam name="T">
        /// Generic type of the parameter.
        /// </typeparam>
        /// <param name="enumerable">
        /// Collection to be checked for null.
        /// </param>
        /// <returns>
        /// Value indicating whether collection is empty or not.
        /// </returns>
        public static bool IsEmpty<T>(this IEnumerable<T> enumerable)
        {
            if (enumerable.IsNull())
            {
                return false;
            }

            return !enumerable.Any();
        }
    }
}