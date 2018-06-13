using System;
using System.Threading.Tasks;
using Pipelines.Implementations;

namespace Pipelines.ExtensionMethods
{
    public static class ActionExtensionMethods
    {
        internal static Func<T, Task> ToAsync<T>(this Action<T> action)
        {
            return args =>
            {
                action(args);
                return Task.CompletedTask;
            };
        }

        public static SafeTypeProcessor<T> ToProcessor<T>(this Func<T, Task> action)
        {
            if (action.HasNoValue())
                return null;

            return ActionProcessor.FromAction<T>(action);
        }

        public static SafeTypeProcessor<T> ToProcessor<T>(this Action<T> action)
        {
            if (action.HasNoValue())
                return null;

            return ActionProcessor.FromAction<T>(action);
        }
    }
}