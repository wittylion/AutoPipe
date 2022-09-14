using System;
using System.Reflection;

namespace AutoPipe
{
    public class DelegateMethodFilter : IMethodFilter
    {
        public DelegateMethodFilter(Predicate<MethodInfo> predicate)
        {
            Predicate = predicate ?? throw new ArgumentNullException(nameof(predicate), "Predicate must be specified.");
        }

        public Predicate<MethodInfo> Predicate { get; }

        public bool Matches(MethodInfo method)
        {
            return Predicate(method);
        }
    }
}