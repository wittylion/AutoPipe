using System.Reflection;

namespace AutoPipe
{
    public interface IMethodFilter
    {
        bool Matches(MethodInfo method);
    }
}
