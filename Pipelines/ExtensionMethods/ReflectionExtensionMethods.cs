using System.Linq;
using System.Reflection;

namespace Pipelines
{
    internal static class ReflectionExtensionMethods
    {
        public static string GetName(this MemberInfo member)
        {
            if (member == null) return "Undefined";

            var nameAttribute = member.GetCustomAttribute<AkaAttribute>();
            var alias = nameAttribute?.Aliases.FirstOrDefault();
            if (alias != null)
            {
                return alias;
            }

            return member.Name;
        }
    }
}
