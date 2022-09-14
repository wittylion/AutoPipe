using System;
using System.Collections.Generic;

namespace AutoPipe
{
    public static class TypeFilter
    {
        public static readonly ITypeFilter Default = new TypeFilterDisjunction(DerivedFrom(), Attributes(), Members(MethodFilterByAttributes.Instance));

        public static ITypeFilter Namespace(string @namespace, bool recursive = true, bool includeSkipped = false)
        {
            return new TypeFilterByNamespace(@namespace, recursive, includeSkipped);
        }

        public static ITypeFilter Attributes(IEnumerable<Type> includingAttributes = null, IEnumerable<Type> excludingAttributes = null)
        {
            return new TypeFilterByAttributes(includingAttributes, excludingAttributes);
        }

        public static ITypeFilter Members(IMethodFilter methodFilter)
        {
            return new TypeFilterByMembers(methodFilter);
        }

        public static ITypeFilter DerivedFrom(Type theType = null, bool includeSkipped = false)
        {
            theType = theType ?? typeof(IProcessor);

            return new DelegateTypeFilter((type) => theType.IsAssignableFrom(type) && (includeSkipped || !type.ShouldSkip()));
        }

        public static ITypeFilter Custom(Predicate<Type> predicate)
        {
            return new DelegateTypeFilter(predicate);
        }
    }
}
