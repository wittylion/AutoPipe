using System.Reflection;

namespace AutoPipe
{
    public class MethodFilter
    {
        public static readonly BindingFlags RunningMethodsFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
    }
}
