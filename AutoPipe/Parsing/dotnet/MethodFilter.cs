using System;
using System.Reflection;

namespace AutoPipe
{
    public class MethodFilter
    {
        public static readonly BindingFlags RunningMethodsFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;

        public static readonly IMethodFilter Default = MethodFilterByAttributes.Instance;
        public static IMethodFilter Custom(Predicate<MethodInfo> predicate)
        {
            return new DelegateMethodFilter(predicate);
        }
    }
}
