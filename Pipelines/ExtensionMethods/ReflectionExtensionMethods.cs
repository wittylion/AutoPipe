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

        public static int GetOrder(this MemberInfo member)
        {
            var orderAttribute = member?.GetCustomAttribute<OrderAttribute>();
            if (orderAttribute != null)
            {
                return orderAttribute.Order;
            }

            return default;
        }

        public static bool ShouldSkip(this MemberInfo member)
        {
            var skipAttribute = member?.GetCustomAttribute<SkipAttribute>();
            return skipAttribute != null;
        }

        public static bool ShouldRun(this MemberInfo member)
        {
            var runAttribute = member?.GetCustomAttribute<RunAttribute>();
            return runAttribute != null;
        }

        public static string GetDescription(this MemberInfo member)
        {
            return member?.GetCustomAttribute<IsAttribute>()?.Description;
        }
    }
}
