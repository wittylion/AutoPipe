using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AutoPipe
{
    internal static class ReflectionExtensionMethods
    {
        public static string GetName(this MemberInfo member)
        {
            return member.GetNames().First();
        }

        public static IEnumerable<string> GetNames(this MemberInfo member)
        {
            if (member == null) return new[] { "Undefined" };

            var nameAttribute = member.GetCustomAttribute<AkaAttribute>();
            var aliases = nameAttribute?.Aliases;
            if (aliases != null && aliases.Any())
            {
                return aliases;
            }

            return new[] { member.Name };
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

        public static bool ShouldRunAll(this MemberInfo member)
        {
            var runAllAttribute = member?.GetCustomAttribute<RunAllAttribute>();
            return runAllAttribute != null;
        }

        public static bool? ClaimAllParameters(this MemberInfo member)
        {
            var runAllAttribute = member?.GetCustomAttribute<RunAttribute>();
            return runAllAttribute?.ClaimAllParameters;
        }

        public static string GetDescription(this MemberInfo member)
        {
            return member?.GetCustomAttribute<IsAttribute>()?.Description;
        }

        public static bool IsProcessor(this Type type)
        {
            return typeof(IProcessor).IsAssignableFrom(type);
        }

        public static bool IsPipeline(this Type type)
        {
            return typeof(IPipeline).IsAssignableFrom(type);
        }

        public static object GetDefault(this Type type)
        {
            if (type.IsValueType)
            {
                return Activator.CreateInstance(type);
            }
            return null;
        }
    }
}
