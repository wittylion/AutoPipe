using System;
using System.Collections.Generic;
using System.Linq;

namespace AutoPipe
{
    public class TypeFilterByAttributes : ITypeFilter
    {
        public IEnumerable<Type> IncludingAttributes { get; }
        public IEnumerable<Type> ExcludingAttributes { get; }

        public TypeFilterByAttributes(IEnumerable<Type> includingAttributes = null, IEnumerable<Type> excludingAttributes = null)
        {

            if (includingAttributes == null && excludingAttributes == null)
            {
                includingAttributes = new[] { typeof(RunAttribute) };
                excludingAttributes = new[] { typeof(SkipAttribute) };
            }

            if ((includingAttributes == null || !includingAttributes.Any()) && (excludingAttributes == null || !excludingAttributes.Any()))
            {
                throw new ArgumentException("At least one collection should have an attribute. Please review the including and excluding collections and add elements.");
            }


            IncludingAttributes = includingAttributes;
            ExcludingAttributes = excludingAttributes;
        }

        public bool Matches(Type type)
        {
            var attributes = type.CustomAttributes;

            if (!ExcludingAttributes.IsEmpty() && attributes.Any(x => ExcludingAttributes.Contains(x.AttributeType)))
            {
                return false;
            }

            if (IncludingAttributes.IsEmpty() || attributes.Any(x => IncludingAttributes.Contains(x.AttributeType)))
            {
                return true;
            }

            return false;
        }
    }
}
