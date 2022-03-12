using System;
using System.Collections.Generic;
using System.Linq;

namespace AutoPipe
{
    public class TypeFilterDisjunction : ITypeFilter
    {
        public TypeFilterDisjunction(params ITypeFilter[] filters)
        {
            Filters = filters.EnsureAtLeastEmpty().OnlyValuable();
        }

        public IEnumerable<ITypeFilter> Filters { get; }

        public bool Matches(Type type)
        {
            return Filters.Any(filter => filter.Matches(type));
        }
    }
}
