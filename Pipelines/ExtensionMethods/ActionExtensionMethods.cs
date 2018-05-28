using System;
using System.Threading.Tasks;

namespace Pipelines.ExtensionMethods
{
    internal static class ActionExtensionMethods
    {
        public static Func<T, Task> ToAsync<T>(this Action<T> action)
        {
            return args =>
            {
                action(args);
                return Task.CompletedTask;
            };
        }
    }
}