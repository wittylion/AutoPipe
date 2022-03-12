using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace AutoPipe
{
    public class TypeFilterConjunction : ITypeFilter
    {
        public TypeFilterConjunction(params ITypeFilter[] filters)
        {
            Filters = filters.EnsureAtLeastEmpty().OnlyValuable();
        }

        public IEnumerable<ITypeFilter> Filters { get; }

        public bool Matches(Type type)
        {
            return Filters.All(filter => filter.Matches(type));
        }
    }
}
