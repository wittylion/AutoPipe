using System;
using System.Linq;

namespace AutoPipe
{
    public class TypeFilterByMembers : ITypeFilter
    {
        public IMethodFilter Filter { get; }

        public TypeFilterByMembers(IMethodFilter methodFilter)
        {
            Filter = methodFilter;
        }

        public bool Matches(Type type)
        {
            return type.GetMethods(MethodFilter.RunningMethodsFlags).Any(member => Filter.Matches(member));
        }
    }
}
