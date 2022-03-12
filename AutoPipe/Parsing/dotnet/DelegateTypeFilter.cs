using System;

namespace AutoPipe
{
    public class DelegateTypeFilter : ITypeFilter
    {
        public static readonly string PredicateShouldBeSpecified = "Using delegate type filter, the predicate that checks type must be specified.";

        public DelegateTypeFilter(Predicate<Type> predicate)
        {
            Predicate = predicate ?? throw new ArgumentNullException(nameof(predicate), PredicateShouldBeSpecified);
        }

        public Predicate<Type> Predicate { get; }

        public bool Matches(Type type)
        {
            var result = Predicate(type);
            return result;
        }
    }
}
