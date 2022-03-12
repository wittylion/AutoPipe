using System.Linq;

namespace AutoPipe
{
    public static class TypeFilterExtensionMethods
    {
        public static ITypeFilter And(this ITypeFilter originalFilter, params ITypeFilter[] alternativeFilters)
        {
            return new TypeFilterConjunction(originalFilter.ToAnArray().Concat(alternativeFilters).ToArray());
        }

        public static ITypeFilter Or(this ITypeFilter originalFilter, params ITypeFilter[] alternativeFilters)
        {
            return new TypeFilterDisjunction(originalFilter.ToAnArray().Concat(alternativeFilters).ToArray());
        }
    }
}
