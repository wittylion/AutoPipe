using System.Reflection;

namespace AutoPipe
{
    public class MethodFilterByAttributes : IMethodFilter
    {
        private static MethodFilterByAttributes _instance;
        public static MethodFilterByAttributes Instance => _instance ?? (_instance = new MethodFilterByAttributes());

        public bool Matches(MethodInfo method)
        {
            if (method.HasNoValue())
            {
                return false;
            }

            return (method.DeclaringType.ShouldRunAll() || method.ShouldRun()) && !method.ShouldSkip();
        }
    }
}
